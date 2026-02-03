// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine;

namespace Mediapipe.Unity
{
#pragma warning disable IDE0065
	using Color = UnityEngine.Color;

#pragma warning restore IDE0065

	public class LineAnnotation : HierarchicalAnnotation
	{
		[SerializeField] private LineRenderer lineRenderer;
		[SerializeField] private Color color = Color.green;
		[SerializeField, Range(0, 1)] private float lineWidth = 1.0f;

		private void OnEnable()
		{
			ApplyColor(color);
			ApplyLineWidth(lineWidth);
		}

		private void OnDisable()
		{
			ApplyLineWidth(0.0f);
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (!UnityEditor.PrefabUtility.IsPartOfAnyPrefab(this))
			{
				ApplyColor(color);
				ApplyLineWidth(lineWidth);
			}
		}
#endif

		public void SetColor(Color col)
		{
			color = col;
			ApplyColor(color);
		}

		public void SetLineWidth(float width)
		{
			lineWidth = width;
			ApplyLineWidth(lineWidth);
		}

		public void Draw(Vector3 a, Vector3 b)
		{
			lineRenderer.SetPositions(new Vector3[] { a, b });
		}

		public void Draw(GameObject a, GameObject b)
		{
			lineRenderer.SetPositions(new Vector3[] { a.transform.localPosition, b.transform.localPosition });
		}

		public void ApplyColor(Color col)
		{
			if (lineRenderer is null) return;
			lineRenderer.startColor = col;
			lineRenderer.endColor = col;
		}

		private void ApplyLineWidth(float width)
		{
			if (lineRenderer is null) return;
			lineRenderer.startWidth = width;
			lineRenderer.endWidth = width;
		}
	}
}