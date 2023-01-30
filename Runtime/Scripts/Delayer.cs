// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Carinnor.XboxController
{ 
    class Delayer
     {
         object script;
         MonoBehaviour mono_script;
         public Delayer(object script)
         {
             this.script = script;
             this.mono_script = this.script as MonoBehaviour;
         }
         public InvokeId DelayExecute(float DelayInSeconds, Action<object[]> lambda, params object[] parameters)
         {
 
            return new InvokeId( mono_script.StartCoroutine(Delayed(DelayInSeconds, lambda, parameters)));
         }
         public InvokeId DelayExecute(float DelayInSeconds, string methodName, params object[] parameters)
         {
             foreach (MethodInfo method in script.GetType().GetMethods())
             {
                 if (method.Name == methodName)
                     return new InvokeId(mono_script.StartCoroutine(Delayed(DelayInSeconds, method, parameters)));
             }
             return null;
         }
         public InvokeId ConditionExecute(Func<bool> condition, string methodName, params object[] parameters)
         {
             foreach (MethodInfo method in script.GetType().GetMethods())
             {
                 if (method.Name == methodName)
                     return new InvokeId(mono_script.StartCoroutine(Delayed(condition, method, parameters)));
             }
             return null;
         }
         public InvokeId ConditionExecute(Func<bool> condition, Action<object[]> lambda, params object[] parameters)
         {
             return new InvokeId(mono_script.StartCoroutine(Delayed(condition, lambda, parameters)));
         }
 
         public void StopExecute(InvokeId id)
         {
             mono_script.StopCoroutine(id.coroutine);
         }
         IEnumerator Delayed(float DelayInSeconds, Action<object[]> lambda, params object[] parameters)
         {
             yield return new WaitForSeconds(DelayInSeconds);
             lambda.Invoke(parameters);
         }
         IEnumerator Delayed(float DelayInSeconds, MethodInfo method, params object[] parameters)
         {
             yield return new WaitForSeconds(DelayInSeconds);
             method.Invoke(script, parameters);
         }
         IEnumerator Delayed(Func<bool> condition, Action<object[]> lambda, params object[] parameters)
         {
             yield return new WaitUntil(condition);
             lambda.Invoke(parameters);
         }
         IEnumerator Delayed(Func<bool> condition, MethodInfo method, params object[] parameters)
         {
             yield return new WaitUntil(condition);
             method.Invoke(script, parameters);
             
         }
 
     }
     class InvokeId
     {
         public readonly Coroutine coroutine;
         public InvokeId(Coroutine coroutine)
         {
             this.coroutine = coroutine;
         }
     }
}