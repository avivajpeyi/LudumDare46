﻿/*
 *Written by Avi Vajpeyi
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Rendering{
	public class VertexData {
		
		public List<Vector3> Vertices = new List<Vector3>();
		public List<Vector3> Normals = new List<Vector3>();
		public List<int> Indices = new List<int>();
	}
}