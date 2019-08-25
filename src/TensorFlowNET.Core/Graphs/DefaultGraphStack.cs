﻿/*****************************************************************************
   Copyright 2018 The TensorFlow.NET Authors. All Rights Reserved.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using static Tensorflow.Binding;

namespace Tensorflow
{

    /// <summary>
    ///     Serves as a stack for determining current default graph.
    /// </summary>
    public class DefaultGraphStack 
    {
        private readonly List<StackModel> stack = new List<StackModel>();

        public void set_controller(Graph @default)
        {
            if (!stack.Exists(x => x.Graph == @default))
                stack.Add(new StackModel {Graph = @default, IsDefault = true});

            foreach (var s in stack)
                s.IsDefault = s.Graph == @default;
        }

        public Graph get_controller()
        {
            if (stack.Count(x => x.IsDefault) == 0)
                stack.Add(new StackModel {Graph = tf.Graph(), IsDefault = true});
            for (var i = stack.Count - 1; i >= 0; i--)
            {
                var x = stack[i];
                if (x.IsDefault)
                    return x.Graph;
            }

            throw new TensorflowException("Unable to find a default graph");
        }

        public bool remove(Graph g)
        {
            if (stack.Count == 0)
                return false;

            var sm = stack.Find(model => model.Graph == g);
            return sm != null && stack.Remove(sm);
        }

        public void reset()
        {
            stack.Clear();
        }

        private class StackModel
        {
            public Graph Graph { get; set; }
            public bool IsDefault { get; set; }
        }
    }
}