﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using AI.Base;

namespace AI
{
    public class ReflectiveDecisionNode : DecisionNode
    {
        [SerializeField] public string MethodName;

        protected override bool IsTrue()
        {
            if(brain == null)
            {
                Debug.LogWarning("No brain");
                return true;
            }
            UnityEngine.Profiling.Profiler.BeginSample("GetReflectiveFunction");
            var f = brain.GetFunction(MethodName);
            UnityEngine.Profiling.Profiler.EndSample();
            return (bool) f.Invoke(brain.Controller,null);
        }
    }
}
