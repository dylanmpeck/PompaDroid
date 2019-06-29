/*
	 * Copyright (c) 2018 Razeware LLC
	 * 
	 * Permission is hereby granted, free of charge, to any person obtaining a copy
	 * of this software and associated documentation files (the "Software"), to deal
	 * in the Software without restriction, including without limitation the rights
	 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	 * copies of the Software, and to permit persons to whom the Software is
	 * furnished to do so, subject to the following conditions:
	 * 
	 * The above copyright notice and this permission notice shall be included in
	 * all copies or substantial portions of the Software.
	 *
	 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
	 * distribute, sublicense, create a derivative work, and/or sell copies of the 
	 * Software in any work that is designed, intended, or marketed for pedagogical or 
	 * instructional purposes related to programming, coding, application development, 
	 * or information technology.  Permission for such use, copying, modification,
	 * merger, publication, distribution, sublicensing, creation of derivative works, 
	 * or sale is expressly withheld.
	 *    
	 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	 * THE SOFTWARE.
	 */


using UnityEngine;

[ExecuteInEditMode]
public class GridSnapper : MonoBehaviour {

	public string filename;
	public bool autoSnapping;

	public Color gridColor = Color.white;

	void Update() {
		if (autoSnapping) {
			SnapChildren();
		}
	}

	public void SnapChildren() {
		foreach (Transform child in transform) {

			//do the snapping;
			Vector3 pos = child.localPosition;
			pos.x = Mathf.RoundToInt(pos.x);
			pos.y = Mathf.RoundToInt(pos.y);
			pos.z = Mathf.RoundToInt(pos.z);
			child.localPosition = pos;
		}
	}

	public Mesh MakeMesh() {
		Mesh mesh = new Mesh();

		int polygons = transform.childCount;

		Vector3[] vertices = new Vector3[polygons * 4];
		Vector2[] uvs = new Vector2[polygons * 4];
		int[] tris = new int[6 * polygons];

		for (int i = 0; i < polygons; i++) {
			SpriteRenderer spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();

			vertices[i * 4 + 0] = spriteRenderer.transform.localPosition + (Vector3)spriteRenderer.sprite.vertices[3];
			vertices[i * 4 + 1] = spriteRenderer.transform.localPosition + (Vector3)spriteRenderer.sprite.vertices[1];
			vertices[i * 4 + 2] = spriteRenderer.transform.localPosition + (Vector3)spriteRenderer.sprite.vertices[0];
			vertices[i * 4 + 3] = spriteRenderer.transform.localPosition + (Vector3)spriteRenderer.sprite.vertices[2];

			uvs[i * 4 + 0] = spriteRenderer.sprite.uv[3];
			uvs[i * 4 + 1] = spriteRenderer.sprite.uv[1];
			uvs[i * 4 + 2] = spriteRenderer.sprite.uv[0];
			uvs[i * 4 + 3] = spriteRenderer.sprite.uv[2];

			tris[i * 6 + 0] = (i * 4) + 0;
			tris[i * 6 + 1] = (i * 4) + 2;
			tris[i * 6 + 2] = (i * 4) + 1;
			tris[i * 6 + 3] = (i * 4) + 2;
			tris[i * 6 + 4] = (i * 4) + 3;
			tris[i * 6 + 5] = (i * 4) + 1;
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = tris;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		return mesh;
	}


	public void OnDrawGizmos() {
		//limit to 20 x 10;

		float scale = 1000;
		int columns = 100;
		int rows = 100;

		Vector2 offset = new Vector2(0.5f, 0.5f);

		Gizmos.color = gridColor;

		for (int j = -rows; j < rows; j++) {


			Vector3 min = new Vector3(scale + offset.x, j + offset.y, 0);
			Vector3 max = new Vector3(-scale + offset.x, j + offset.y, 0);

			min = transform.TransformPoint(min);
			max = transform.TransformPoint(max);

			Gizmos.DrawLine(min, max);

		}

		for (int i = -columns; i < columns; i++) {

			Vector3 min = new Vector3(i + offset.x, +scale + offset.y, 0);
			Vector3 max = new Vector3(i + offset.x, -scale + offset.y, 0);

			min = transform.TransformPoint(min);
			max = transform.TransformPoint(max);

			Gizmos.DrawLine(min, max);

		}




	}
}
