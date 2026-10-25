using System.Linq;
using Godot;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesGodot
{
    public static bool EqualsByType(this Variant left, Variant right)
    {
        if (left.VariantType != right.VariantType)
            return false;

        return left.VariantType switch
        {
            Variant.Type.Nil => true,
            Variant.Type.Bool => left.AsBool() == right.AsBool(),
            Variant.Type.Int => left.AsInt64() == right.AsInt64(),
            Variant.Type.Float => left.AsDouble().Equals(right.AsDouble()),
            Variant.Type.String => left.AsString() == right.AsString(),
            Variant.Type.Vector2 => left.AsVector2().Equals(right.AsVector2()),
            Variant.Type.Vector2I => left.AsVector2I().Equals(right.AsVector2I()),
            Variant.Type.Rect2 => left.AsRect2().Equals(right.AsRect2()),
            Variant.Type.Rect2I => left.AsRect2I().Equals(right.AsRect2I()),
            Variant.Type.Vector3 => left.AsVector3().Equals(right.AsVector3()),
            Variant.Type.Vector3I => left.AsVector3I().Equals(right.AsVector3I()),
            Variant.Type.Transform2D => left.AsTransform2D().Equals(right.AsTransform2D()),
            Variant.Type.Vector4 => left.AsVector4().Equals(right.AsVector4()),
            Variant.Type.Vector4I => left.AsVector4I().Equals(right.AsVector4I()),
            Variant.Type.Plane => left.AsPlane().Equals(right.AsPlane()),
            Variant.Type.Quaternion => left.AsQuaternion().Equals(right.AsQuaternion()),
            Variant.Type.Aabb => left.AsAabb().Equals(right.AsAabb()),
            Variant.Type.Basis => left.AsBasis().Equals(right.AsBasis()),
            Variant.Type.Transform3D => left.AsTransform3D().Equals(right.AsTransform3D()),
            Variant.Type.Projection => left.AsProjection().Equals(right.AsProjection()),
            Variant.Type.Color => left.AsColor().Equals(right.AsColor()),
            Variant.Type.StringName => left.AsStringName().Equals(right.AsStringName()),
            Variant.Type.NodePath => left.AsNodePath().Equals(right.AsNodePath()),
            Variant.Type.Rid => left.AsRid().Equals(right.AsRid()),
            Variant.Type.Object => ReferenceEquals(left.AsGodotObject(), right.AsGodotObject()),
            Variant.Type.Callable => left.AsCallable().Equals(right.AsCallable()),
            Variant.Type.Signal => left.AsSignal().Equals(right.AsSignal()),
            Variant.Type.Dictionary => DictionaryEquals(left.AsGodotDictionary(), right.AsGodotDictionary()),
            Variant.Type.Array => ArrayEquals(left.AsGodotArray(), right.AsGodotArray()),
            Variant.Type.PackedByteArray => left.AsByteArray().SequenceEqual(right.AsByteArray()),
            Variant.Type.PackedInt32Array => left.AsInt32Array().SequenceEqual(right.AsInt32Array()),
            Variant.Type.PackedInt64Array => left.AsInt64Array().SequenceEqual(right.AsInt64Array()),
            Variant.Type.PackedFloat32Array => left.AsFloat32Array().SequenceEqual(right.AsFloat32Array()),
            Variant.Type.PackedFloat64Array => left.AsFloat64Array().SequenceEqual(right.AsFloat64Array()),
            Variant.Type.PackedStringArray => left.AsStringArray().SequenceEqual(right.AsStringArray()),
            Variant.Type.PackedVector2Array => left.AsVector2Array().SequenceEqual(right.AsVector2Array()),
            Variant.Type.PackedVector3Array => left.AsVector3Array().SequenceEqual(right.AsVector3Array()),
            Variant.Type.PackedColorArray => left.AsColorArray().SequenceEqual(right.AsColorArray()),
            Variant.Type.PackedVector4Array => left.AsVector4Array().SequenceEqual(right.AsVector4Array()),

            // Max no debe evaluarse
            Variant.Type.Max => throw new System.InvalidOperationException("Tipo Variant inválido"),

            _ => throw new System.NotImplementedException($"Comparación no implementada para {left.VariantType}")
        };
    }

    // -------- Helpers --------

    private static bool DictionaryEquals(Godot.Collections.Dictionary left, Godot.Collections.Dictionary right)
    {
        if (left.Count != right.Count)
            return false;

        foreach (var key in left.Keys)
        {
            if (!right.ContainsKey(key))
                return false;

            if (!((Variant)left[key]).EqualsByType((Variant)right[key]))
                return false;
        }
        return true;
    }

    private static bool ArrayEquals(Godot.Collections.Array left, Godot.Collections.Array right)
    {
        if (left.Count != right.Count)
            return false;

        for (int i = 0; i < left.Count; i++)
        {
            if (!((Variant)left[i]).EqualsByType((Variant)right[i]))
                return false;
        }

        return true;
    }
}
