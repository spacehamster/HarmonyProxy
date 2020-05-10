using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	public class PatchProcessor
	{
		static object locker = new object();
		List<HarmonyLib.PatchProcessor> patchProcessors;
		readonly HarmonyInstance instance;

		readonly Type container;
		readonly HarmonyMethod containerAttributes;

		List<MethodBase> originals = new List<MethodBase>();
		HarmonyMethod prefix;
		HarmonyMethod postfix;
		HarmonyMethod transpiler;
		public PatchProcessor(HarmonyInstance instance, Type type, HarmonyMethod attributes)
		{
			this.instance = instance;
			container = type;
			containerAttributes = attributes ?? new HarmonyMethod(null);
			prefix = containerAttributes.Clone();
			postfix = containerAttributes.Clone();
			transpiler = containerAttributes.Clone();
			PrepareType();
			patchProcessors = new List<HarmonyLib.PatchProcessor>();
			foreach(var method in originals)
			{
				patchProcessors.Add(new HarmonyLib.PatchProcessor(instance.instance, method));
			}
			if (prefix?.method != null) patchProcessors.Do(p => p.AddPrefix(prefix.ToHarmony20()));
			if (postfix?.method != null) patchProcessors.Do(p => p.AddPostfix(postfix.ToHarmony20()));
			if (transpiler?.method != null) patchProcessors.Do(p => p.AddTranspiler(transpiler.ToHarmony20()));
		}

		public PatchProcessor(HarmonyInstance instance, List<MethodBase> originals, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
		{
			patchProcessors = new List<HarmonyLib.PatchProcessor>();
			foreach (var method in originals)
			{
				patchProcessors.Add(new HarmonyLib.PatchProcessor(instance.instance, method));
			}
			this.instance = instance;
			this.originals = originals;
			if (prefix?.method != null) patchProcessors.Do(p => p.AddPrefix(prefix.ToHarmony20()));
			if (postfix?.method != null) patchProcessors.Do(p => p.AddPostfix(postfix.ToHarmony20()));
			if (transpiler?.method != null) patchProcessors.Do(p => p.AddTranspiler(transpiler.ToHarmony20()));
			this.prefix = prefix ?? new HarmonyMethod(null);
			this.postfix = postfix ?? new HarmonyMethod(null);
			this.transpiler = transpiler ?? new HarmonyMethod(null);
		}

		public static Patches GetPatchInfo(MethodBase method)
		{
			return HarmonyLib.PatchProcessor.GetPatchInfo(method).ToHarmony12();
		}

		public static IEnumerable<MethodBase> AllPatchedMethods()
		{
			return HarmonyLib.PatchProcessor.GetAllPatchedMethods();
		}

		public List<DynamicMethod> Patch()
		{
			//TODO: What happens if patch doesn't return a dynamic method?
			var result = new List<DynamicMethod>();
			foreach (var pp in patchProcessors)
			{
				var info = pp.Patch();
				if (info is DynamicMethod dm)
				{
					result.Add(dm);
				}
				else
				{
					throw new Exception($"Invalid return type {info}");
				}
			}
			return result;
		}

		public void Unpatch(HarmonyPatchType type, string harmonyID)
		{
			patchProcessors.Do(p => p.Unpatch(type.ToHarmony20(), harmonyID));
		}

		public void Unpatch(MethodInfo patch)
		{
			patchProcessors.Do(p => p.Unpatch(patch));
		}

		void PrepareType()
		{
			var mainPrepareResult = RunMethod<HarmonyPrepare, bool>(true);
			if (mainPrepareResult == false)
				return;

			var customOriginals = RunMethod<HarmonyTargetMethods, IEnumerable<MethodBase>>(null);
			if (customOriginals != null)
			{
				originals = customOriginals.ToList();
			}
			else
			{
				var originalMethodType = containerAttributes.methodType;

				// MethodType default is Normal
				if (containerAttributes.methodType == null)
					containerAttributes.methodType = MethodType.Normal;

				var isPatchAll = Attribute.GetCustomAttribute(container, typeof(HarmonyPatchAll)) != null;
				if (isPatchAll)
				{
					var type = containerAttributes.declaringType;
					originals.AddRange(AccessTools.GetDeclaredConstructors(type).Cast<MethodBase>());
					originals.AddRange(AccessTools.GetDeclaredMethods(type).Cast<MethodBase>());
				}
				else
				{
					var original = RunMethod<HarmonyTargetMethod, MethodBase>(null);

					if (original == null)
						original = GetOriginalMethod();

					if (original == null)
					{
						var info = "(";
						info += "declaringType=" + containerAttributes.declaringType + ", ";
						info += "methodName =" + containerAttributes.methodName + ", ";
						info += "methodType=" + originalMethodType + ", ";
						info += "argumentTypes=" + containerAttributes.argumentTypes.Description();
						info += ")";
						throw new ArgumentException("No target method specified for class " + container.FullName + " " + info);
					}

					originals.Add(original);
				}
			}

			PatchTools.GetPatches(container, out prefix.method, out postfix.method, out transpiler.method);

			if (prefix.method != null)
			{
				if (prefix.method.IsStatic == false)
					throw new ArgumentException("Patch method " + prefix.method.FullDescription() + " must be static");

				var prefixAttributes = prefix.method.GetHarmonyMethods();
				containerAttributes.Merge(HarmonyMethod.Merge(prefixAttributes)).CopyTo(prefix);
			}

			if (postfix.method != null)
			{
				if (postfix.method.IsStatic == false)
					throw new ArgumentException("Patch method " + postfix.method.FullDescription() + " must be static");

				var postfixAttributes = postfix.method.GetHarmonyMethods();
				containerAttributes.Merge(HarmonyMethod.Merge(postfixAttributes)).CopyTo(postfix);
			}

			if (transpiler.method != null)
			{
				if (transpiler.method.IsStatic == false)
					throw new ArgumentException("Patch method " + transpiler.method.FullDescription() + " must be static");

				var infixAttributes = transpiler.method.GetHarmonyMethods();
				containerAttributes.Merge(HarmonyMethod.Merge(infixAttributes)).CopyTo(transpiler);
			}
		}

		MethodBase GetOriginalMethod()
		{
			var attr = containerAttributes;
			if (attr.declaringType == null) return null;

			switch (attr.methodType)
			{
				case MethodType.Normal:
					if (attr.methodName == null)
						return null;
					return AccessTools.DeclaredMethod(attr.declaringType, attr.methodName, attr.argumentTypes);

				case MethodType.Getter:
					if (attr.methodName == null)
						return null;
					return AccessTools.DeclaredProperty(attr.declaringType, attr.methodName).GetGetMethod(true);

				case MethodType.Setter:
					if (attr.methodName == null)
						return null;
					return AccessTools.DeclaredProperty(attr.declaringType, attr.methodName).GetSetMethod(true);

				case MethodType.Constructor:
					return AccessTools.DeclaredConstructor(attr.declaringType, attr.argumentTypes);

				case MethodType.StaticConstructor:
					return AccessTools.GetDeclaredConstructors(attr.declaringType)
						.Where(c => c.IsStatic)
						.FirstOrDefault();
			}

			return null;
		}

		T RunMethod<S, T>(T defaultIfNotExisting, params object[] parameters)
		{
			if (container == null)
				return defaultIfNotExisting;

			var methodName = typeof(S).Name.Replace("Harmony", "");

			var paramList = new List<object> { instance };
			paramList.AddRange(parameters);
			var paramTypes = AccessTools.GetTypes(paramList.ToArray());
			var method = PatchTools.GetPatchMethod<S>(container, methodName, paramTypes);
			if (method != null && typeof(T).IsAssignableFrom(method.ReturnType))
				return (T)method.Invoke(null, paramList.ToArray());

			method = PatchTools.GetPatchMethod<S>(container, methodName, new Type[] { typeof(HarmonyInstance) });
			if (method != null && typeof(T).IsAssignableFrom(method.ReturnType))
				return (T)method.Invoke(null, new object[] { instance });

			method = PatchTools.GetPatchMethod<S>(container, methodName, Type.EmptyTypes);
			if (method != null)
			{
				if (typeof(T).IsAssignableFrom(method.ReturnType))
					return (T)method.Invoke(null, Type.EmptyTypes);

				method.Invoke(null, Type.EmptyTypes);
				return defaultIfNotExisting;
			}

			return defaultIfNotExisting;
		}

		void RunMethod<S>(params object[] parameters)
		{
			if (container == null)
				return;

			var methodName = typeof(S).Name.Replace("Harmony", "");

			var paramList = new List<object> { instance };
			paramList.AddRange(parameters);
			var paramTypes = AccessTools.GetTypes(paramList.ToArray());
			var method = PatchTools.GetPatchMethod<S>(container, methodName, paramTypes);
			if (method != null)
			{
				method.Invoke(null, paramList.ToArray());
				return;
			}

			method = PatchTools.GetPatchMethod<S>(container, methodName, new Type[] { typeof(HarmonyInstance) });
			if (method != null)
			{
				method.Invoke(null, new object[] { instance });
				return;
			}

			method = PatchTools.GetPatchMethod<S>(container, methodName, Type.EmptyTypes);
			if (method != null)
			{
				method.Invoke(null, Type.EmptyTypes);
				return;
			}
		}
	}
}