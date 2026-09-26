using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace BlobEatBlob.HelperScripts;

    public static class Node2DExtensions
    {
        public static bool DisableIfMissing(this Node node, GodotObject value,
            [CallerArgumentExpression(nameof(value))]
            string name = "")
        {
            if (value is not null) return false;
            GD.PushError($"{name} is not assigned.");
            node.SetProcess(false);
            return true;
        }
    }
