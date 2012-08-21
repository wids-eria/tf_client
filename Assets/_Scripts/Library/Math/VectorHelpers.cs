using UnityEngine;
using System.Collections;

/// <file>
/// 
/// <author>
/// Adam Mechtley
/// http://adammechtley.com/donations
/// </author>
/// 
/// <copyright>
/// Copyright (c) 2011,  Adam Mechtley.
/// All rights reserved.
/// 
/// Redistribution and use in source and binary forms, with or without
/// modification, are permitted provided that the following conditions are met:
/// 
/// 1. Redistributions of source code must retain the above copyright notice,
/// this list of conditions and the following disclaimer.
/// 
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
/// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
/// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
/// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
/// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
/// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
/// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
/// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
/// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
/// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
/// POSSIBILITY OF SUCH DAMAGE.
/// </copyright>
/// 
/// <summary>
/// This file contains a class with static methods for working with Vectors.
/// </summary>
/// 
/// </file>

/// <summary>
/// an enum for choosing how two axes should be compared
/// </summary>
public enum VectorComparisonMode
{
	Dot, // faster computation, e.g., if only sign is needed
	Angle // constant angular velocity
}

/// <summary>
/// an anum for choosing how two vectors should be interpolated
/// </summary>
public enum VectorInterpolationMode
{
	Lerp, // faster computation
	Slerp // constaint angular velocity
}

/// <summary>
/// A class for working with Vectors
/// </summary>
public static class VectorHelpers : System.Object
{
	/// <summary>
	/// an array containing all of the cardinal axes
	/// </summary>
	public static readonly Vector3[] cardinalAxes = new Vector3[6]
	{
		Vector3.right, Vector3.up, Vector3.forward,
		Vector3.left, Vector3.down, Vector3.back
	};
	
	/// <summary>
	/// Find the cardinal axis that is nearest to testVector
	/// </summary>
	/// <param name="testVector">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public static Vector3 FindNearestCardinalAxis(Vector3 testVector)
	{
		testVector.Normalize();
		Vector3 nearest = Vector3.forward;
		Vector3[] cardinals = cardinalAxes;
		for (int i=0; i<cardinals.Length; i++)
			if (Vector3.Dot(testVector,cardinals[i])>Vector3.Dot(testVector,nearest))
				nearest = cardinals[i];
		return nearest;
	}
	
	/// <summary>
	/// Mirror point p across the plane defined by the normal n
	/// </summary>
	/// <param name="p">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="n">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public static Vector3 MirrorPointAcrossPlane(Vector3 p, Vector3 n)
	{
		n.Normalize();
		return p-2f*(Vector3.Dot(p, n)*n);
	}
	
	/// <summary>
	/// Distance for a plane intersection from the origin
	/// </summary>
	private static float _intersectionDistance;
	
	/// <summary>
	/// Get the intersection point of a line direction, passing through origin, and a plane
	/// </summary>
	/// <param name="origin">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="direction">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="p">
	/// A <see cref="Plane"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public static bool GetIntersectionOnPlane(Vector3 origin, Vector3 direction, Plane p, out Vector3 intersectionPoint)
	{
		return GetIntersectionOnPlane(new Ray(origin, direction), p, out intersectionPoint);
	}
	/// <summary>
	/// Get the intersection point of a ray and a plane
	/// </summary>
	/// <param name="ray">
	/// A <see cref="Ray"/>
	/// </param>
	/// <param name="p">
	/// A <see cref="Plane"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public static bool GetIntersectionOnPlane(Ray ray, Plane p, out Vector3 intersectionPoint)
	{
		bool ret = p.Raycast(ray, out _intersectionDistance);
		intersectionPoint = ray.GetPoint(_intersectionDistance);
		return ret;
	}
	
	/// <summary>
	/// Zeros the Y component.
	/// </summary>
	/// <returns>
	/// The Y component.
	/// </returns>
	/// <param name='p'>
	/// P.
	/// </param>
	public static Vector3 ZeroYComponent(Vector3 p)
	{
		p.y = 0f;
		return p;
	}
	
	/// <summary>
	/// Return the smallest element in a vector
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector2"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	public static float MinValue(Vector2 v)
	{
		return Mathf.Min(v.x, v.y);
	}
	/// <summary>
	/// Return the smallest element in a vector
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	public static float MinValue(Vector3 v)
	{
		return Mathf.Min(Mathf.Min(v.x, v.y), Mathf.Min(v.y, v.z));
	}
	/// <summary>
	/// Return the smallest element in a vector
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector4"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	public static float MinValue(Vector4 v)
	{
		return Mathf.Min(Mathf.Min(v.x, v.y), Mathf.Min(v.z, v.w));
	}
	
	/// <summary>
	/// Return the largest element in a vector
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector2"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	public static float MaxValue(Vector2 v)
	{
		return Mathf.Max(v.x, v.y);
	}
	/// <summary>
	/// Return the smallest element in a vector
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	public static float MaxValue(Vector3 v)
	{
		return Mathf.Max(Mathf.Max(v.x, v.y), Mathf.Max(v.y, v.z));
	}
	/// <summary>
	/// Return the smallest element in a vector
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector4"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	public static float MaxValue(Vector4 v)
	{
		return Mathf.Max(Mathf.Max(v.x, v.y), Mathf.Max(v.z, v.w));
	}
	
	/// <summary>
	/// Scale a vector using another scale vector
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="scale">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public static Vector3 ScaleByVector(Vector3 v, Vector3 scale)
	{
		return new Vector3(v.x*scale.x, v.y*scale.y, v.z*scale.z);
	}
	
	/// <summary>
	/// Create an axis tripod from a supplied up-vector
	/// </summary>
	/// <param name="up">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="AxisTripod"/>
	/// </returns>
	public static AxisTripod AxisTripodFromUp(Vector3 up)
	{
		AxisTripod tripod = new AxisTripod();
		tripod.up = up.normalized;
		tripod.right = Vector3.right;
		float dot = Vector3.Dot(tripod.up, tripod.right);
		if (dot==1f) tripod.right = Vector3.forward;
		else if (dot==-1f) tripod.right = Vector3.back;
		tripod.forward = Vector3.Cross(tripod.right, tripod.up).normalized;
		tripod.right = Vector3.Cross(tripod.up, tripod.forward);
		return tripod;
	}
	
	/// <summary>
	/// Create an axis tripod from a supplied forward- and up-axis
	/// </summary>
	/// <param name="forward">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <param name="up">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="AxisTripod"/>
	/// </returns>
	public static AxisTripod AxisTripodFromForwardUp(Vector3 forward, Vector3 up)
	{
		AxisTripod tripod = new AxisTripod();
		Quaternion q = Quaternion.LookRotation(forward, up);
		tripod.forward = q*Vector3.forward;
		tripod.up = q*Vector3.up;
		tripod.right = q*Vector3.right;
		return tripod;
	}
	
	/// <summary>
	/// Create an axis tripod from a Quaternion
	/// </summary>
	/// <param name="quat">
	/// A <see cref="Quaternion"/>
	/// </param>
	/// <returns>
	/// A <see cref="AxisTripod"/>
	/// </returns>
	public static AxisTripod AxisTripodFromQuaternion(Quaternion quat)
	{
		return AxisTripodFromForwardUp(quat*Vector3.forward, quat*Vector3.up);
	}
	
	/// <summary>
	/// Delimeter for vector serialization
	/// </summary>
	public static char serializationDelimter = ","[0];
	/// <summary>
	/// Serialize a Vector2 as a string
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector2"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public static string SerializeString(Vector2 v)
	{
		return string.Format("{1}{0}{2}", serializationDelimter, v.x, v.y);
	}
	/// <summary>
	/// Serialize a Vector3 as a string
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public static string SerializeString(Vector3 v)
	{
		return string.Format("{1}{0}{2}{0}{3}", serializationDelimter, v.x, v.y, v.z);
	}
	/// <summary>
	/// Serialize a Vector4 as a string
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector4"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public static string SerializeString(Vector4 v)
	{
		return string.Format("{1}{0}{2}{0}{3}{0}{4}", serializationDelimter, v.x, v.y, v.z, v.w);
	}
	/// <summary>
	/// Deserialize a Vector2 string
	/// </summary>
	/// <param name="s">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector2"/>
	/// </returns>
	public static Vector2 DeserializeVector2String(string s)
	{
		string[] tokens = s.Split(serializationDelimter);
		return new Vector2(float.Parse(tokens[0]), float.Parse(tokens[1]));
	}
	/// <summary>
	/// Deserialize a Vector3 string
	/// </summary>
	/// <param name="s">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public static Vector3 DeserializeVector3String(string s)
	{
		string[] tokens = s.Split(serializationDelimter);
		return new Vector3(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]));
	}
	/// <summary>
	/// Deserialize a Vector4 string
	/// </summary>
	/// <param name="s">
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector4"/>
	/// </returns>
	public static Vector4 DeserializeVector4String(string s)
	{
		string[] tokens = s.Split(serializationDelimter);
		return new Vector4(float.Parse(tokens[0]), float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
	}
}

/// <summary>
/// A struct to describe a left-handed axis tripod of three orthonormalized vectors
/// </summary>
public struct AxisTripod
{
	/// <summary>
	/// right axis
	/// </summary>
	public Vector3 right;
	/// <summary>
	/// left axis
	/// </summary>
	public Vector3 left
	{
		get { return -right; }
		set { right = -value; }
	}
	/// <summary>
	/// up axis
	/// </summary>
	public Vector3 up;
	/// <summary>
	/// down axis
	/// </summary>
	public Vector3 down
	{
		get { return -up; }
		set { up = -value; }
	}
	/// <summary>
	/// forward axis
	/// </summary>
	public Vector3 forward;
	/// <summary>
	/// back axis
	/// </summary>
	public Vector3 back
	{
		get { return -forward; }
		set { forward = -value; }
	}
}