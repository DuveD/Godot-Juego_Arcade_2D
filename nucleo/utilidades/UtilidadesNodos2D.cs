
using Godot;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesNodos2D
{
    internal static void AjustarZIndexNodo(Node2D nodo2D, int zIndex)
    {
        nodo2D.ZIndex = zIndex;
        nodo2D.ZAsRelative = false;
    }

    public static bool Solape(Node2D nodoA, Node2D nodoB)
    {
        if (nodoA is not CollisionObject2D collisionObjectA || nodoB is not CollisionObject2D collisionObjectB)
            return false;

        foreach (uint shapeA in collisionObjectA.GetShapeOwners())
        {
            foreach (uint shapeB in collisionObjectB.GetShapeOwners())
            {
                var shapeOwnerA = collisionObjectA.ShapeOwnerGetShape(shapeA, 0);
                var shapeOwnerB = collisionObjectB.ShapeOwnerGetShape(shapeB, 0);

                if (shapeOwnerA == null || shapeOwnerB == null)
                    continue;

                if (shapeOwnerA.Collide(
                        collisionObjectA.GlobalTransform,
                        shapeOwnerB,
                        collisionObjectB.GlobalTransform))
                    return true;
            }
        }

        return false;
    }
}
