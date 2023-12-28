using System.Numerics;
using ExtensionMethods;
using Raylib_cs;
using LanguageExt;
using static LanguageExt.Prelude;

namespace ProtoPlat;

public struct RaycastResult
{
    public Vector2 ContactPoint;
    public Vector2 ContactNormal;
    public float DeltaHitNear;
}

public class DynamicCollisionResult
{
    public Vector2 ContactPoint;
    public Vector2 ContactNormal;
    public float ContactDelta;
}

public static class CollisionUtils
{
    public static Option<RaycastResult> RayVsRect2D(Vector2 rayOrigin, Vector2 rayDirection, Rectangle targetRect)
    {
        float nearContactTime = 0f;
        Vector2 contactNormal = Vector2.Zero;
            
        /*
        * The t in the P(t) = P + D.t
        * Where t is the parametric variable to plot the near collison point using the parametric line equation (P(t) = P + D.t)
        * Where P is the Position Vector of the Ray and D is the Direciton Vector of the Ray
        */
        float tHitNear = 0;

        /*
        * Calculate intersection points with rectangle bounding axes
        * Parametric 't' for Near (X,Y) and Far (X,Y)
        */
        float deltaT1X = targetRect.X - rayOrigin.X;
        float tHitNearX = (deltaT1X / rayDirection.X);

        float deltaT1Y = targetRect.Y - rayOrigin.Y;
        float tHitNearY = (deltaT1Y / rayDirection.Y);

        float deltaT2X = targetRect.X + targetRect.Width - rayOrigin.X;
        float tHitFarX = (deltaT2X / rayDirection.X);

        float deltaT2Y = targetRect.Y + targetRect.Height - rayOrigin.Y;
        float tHitFarY = (deltaT2Y / rayDirection.Y);

        /*
        * Sort the distances to maintain Affine uniformity
        * i.e. sort the near and far axes of the rectangle in the appropritate order from the POV of ray_origin
        */
        if (tHitNearX > tHitFarX) (tHitNearX, tHitFarX) = (tHitFarX, tHitNearX);
        if (tHitNearY > tHitFarY) (tHitNearY, tHitFarY) = (tHitFarY, tHitNearY);

        // As there is no chance of collision i.e the paramteric line cannot pass throguh the rectangle the probable points are empty
        // probableContactPoints[0] = (Vector2){0, 0};
        // probableContactPoints[1] = (Vector2){0, 0};

        /*
        * Check the order of the near and far points
        * if they satisfy the below condition the line will pass through the rectanle (It didn't yet)
        * if not return out of the function as it will not pass through
        */
        if(!(tHitNearX < tHitFarY && tHitNearY < tHitFarX)) return None;

        /*
        * If the parametric line passes through the rectangle calculate the parametric 't'
        * the 't' should be such that it must lie on both the Line/Ray and the Rectangle
        * Use the condition below to calculate the 'tnear' and 'tfar' that gives the near and far collison parametric t
        */
        nearContactTime = Math.Max(tHitNearX, tHitNearY);
        float tHitFar = Math.Min(tHitFarX, tHitFarY);

        // Use the parametric t values calculated above and subsitute them in the parametric equation to get the near and far contact points
        float hitNearXPosition = rayOrigin.X + (rayDirection.X * nearContactTime);
        float hitNearYPosition = rayOrigin.Y + (rayDirection.Y * nearContactTime);

        // float hitFarXPosition = rayOrigin.X + (rayDirection.X * tHitFar);
        // float hitFarYPosition = rayOrigin.Y + (rayDirection.Y * tHitFar);

        // // Generate Vectors using the near and far collision points
        // Vector2 nearHitVector = new Vector2(hitNearXPosition, hitNearYPosition);
        // Vector2 farHitVector = new Vector2(hitFarXPosition, hitFarYPosition);
        // // Since we are sure that the line will pass through the rectanle upadte the probable near and far points
        // probableContactPoints[0] = nearHitVector;
        // probableContactPoints[1] = farHitVector;

        /*
        * Check wether the parametric 't' values are withing certain bounds to guarantee that the probable collision has actually occured
        * i.e. If the below conditions are true only then the Ray has passed through the Rectangle
        * if not it still can pass but it did not yet
        */
        if(tHitFar < 0 || tHitNear > 1) return None;

        // Now Update the actual contact point
        var contactPoint = new Vector2(hitNearXPosition, hitNearYPosition);

        // Update the contact normal of the Ray with the Rectangle surface
        if(tHitNearX > tHitNearY)
            contactNormal = rayDirection.X < 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
        else if(tHitNearX < tHitNearY)
            contactNormal = rayDirection.Y < 0 ? new Vector2(0, 1) : new Vector2(0, -1);
        // Since the contact has definitely occured return true
        return new RaycastResult
        {
            ContactPoint = contactPoint,
            ContactNormal = contactNormal,
            DeltaHitNear = nearContactTime
        };
    }
    
    public static Option<RaycastResult> Raycast(Vector2 rayOrigin, Vector2 rayDirection, Rectangle target, float maxDistance = 0f)
    {
        var contactNormal = Vector2.Zero;
        var deltaHitNear = 0f;

        // Cache division
        var invDir = 1f.Divide(rayDirection);

        var targetPos = new Vector2(target.X, target.Y);
        var targetSize = new Vector2(target.Width, target.Height);

        // Calculate intersections with rectangle bounding axes
        var deltaNear = (targetPos - rayOrigin) * invDir;
        var deltaFar = (targetPos + targetSize - rayOrigin) * invDir;

        // Sort distances
        if (deltaNear.X > deltaFar.X) (deltaNear.X, deltaFar.X) = (deltaFar.X, deltaNear.X);
        if (deltaNear.Y > deltaFar.Y) (deltaNear.Y, deltaFar.Y) = (deltaFar.Y, deltaNear.Y);

        // Early rejection
        if (deltaNear.X > deltaFar.Y || deltaNear.Y > deltaFar.X) return None;

        // Closest 'time' will be the first contact
        deltaHitNear = Math.Max(deltaNear.X, deltaNear.Y);
        
        // Furthest 'time' is contact on opposite side of target
        var deltaHitFar = Math.Min(deltaFar.X, deltaFar.Y);
        
        // Reject if ray direction is pointing away from object
        if (deltaHitFar < 0)
            return None;
        
        // Contact point of collision from parametric line equation
        var contactPoint = rayOrigin + deltaHitNear * rayDirection;
        
        if (deltaNear.X > deltaNear.Y)
            contactNormal = invDir.X < 0 ? new Vector2 (1, 0) : new Vector2(-1, 0);
        else if (deltaNear.X < deltaNear.Y) contactNormal = invDir.Y < 0 ? new Vector2(0, 1) : new Vector2(0, -1);

        // Note if deltaNear == deltaFar, collision is principly in a diagonal
        // so pointless to resolve. By returning a CN={0,0} even though its
        // considered a hit, the resolver wont change anything.
        return new RaycastResult
        {
            ContactPoint = contactPoint,
            ContactNormal = contactNormal,
            DeltaHitNear = deltaHitNear
        };
    }

    public static Option<DynamicCollisionResult> DynamicRectCollisionCheck(Rectangle dynamicRect, Vector2 dynamicVel, Rectangle staticRect, float delta)
    {
        var dynPos = new Vector2(dynamicRect.X, dynamicRect.Y);
        var dynSize = new Vector2(dynamicRect.Width, dynamicRect.Height);
        var staticPos = new Vector2(staticRect.X, staticRect.Y);
        var staticSize = new Vector2(staticRect.Width, staticRect.Height);

        // Check if dynamic rectangle is actually moving - we assume rectangles are NOT in collision to start
        if (dynamicVel == Vector2.Zero)
            return None;
        
        Option<DynamicCollisionResult> mCollisionResult = None;

        Rectangle expandedTarget;

        // Expand target rectangle by source dimensions
        var exPos = staticPos - dynSize / 2;
        var exSize = staticSize + dynSize;
        expandedTarget.X = exPos.X;
        expandedTarget.Y = exPos.Y;
        expandedTarget.Width = exSize.X;
        expandedTarget.Height = exSize.Y;

        var rayResult = Raycast(dynPos + dynSize / 2, dynamicVel * delta, expandedTarget);
        rayResult.IfSome(result =>
        {
            if (result.DeltaHitNear is >= 0f and < 1f)
                mCollisionResult = new DynamicCollisionResult
                {
                    ContactPoint = result.ContactPoint,
                    ContactNormal = result.ContactNormal,
                    ContactDelta = result.DeltaHitNear
                };
        });

        return mCollisionResult;
    }

    public static bool ResolveDynamicCollision(Rectangle dynamicRect, Vector2 dynamicVel, Rectangle staticRect, float delta)
    {
        var result = false;
        var contactPoint = Vector2.Zero;
        var contactnormal = Vector2.Zero;
        var contactDelta = 0f;

        var dynamicCollisionResult = DynamicRectCollisionCheck(dynamicRect, dynamicVel, staticRect, delta);
        dynamicCollisionResult.IfSome(colResult =>
        {
            
        });

        return result;
    }

    // bool ResolveDynamicRectVsRect(olc::aabb::rect* r_dynamic, const float fTimeStep, olc::aabb::rect* r_static)
    // {
    //     olc::vf2d contact_point, contact_normal;
    //     float contact_time = 0.0f;
    //     if (DynamicRectVsRect(r_dynamic, fTimeStep, *r_static, contact_point, contact_normal, contact_time))
    //     {
    //         if (contact_normal.y > 0) r_dynamic->contact[0] = r_static; else nullptr;
    //         if (contact_normal.x < 0) r_dynamic->contact[1] = r_static; else nullptr;
    //         if (contact_normal.y < 0) r_dynamic->contact[2] = r_static; else nullptr;
    //         if (contact_normal.x > 0) r_dynamic->contact[3] = r_static; else nullptr;
    //
    //         r_dynamic->vel += contact_normal * olc::vf2d(std::abs(r_dynamic->vel.x), std::abs(r_dynamic->vel.y)) * (1 - contact_time);
    //         return true;
    //     }
    //
    //     return false;
    // }
}