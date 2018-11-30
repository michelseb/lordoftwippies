using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPU_colorized : MonoBehaviour {

	public struct Vert{
		public Vector3 pos;
		public Color Couleur;
		public Vert( Vector3 pos, Color Couleur){
			this.pos = pos;
			this.Couleur = Couleur;
		}
	}

	public struct Cent{
		public Vector3 pos;
		public Color Couleur;
		public Cent( Vector3 pos, Color Couleur){
			this.pos = pos;
			this.Couleur = Couleur;
		}
	}

	public ComputeShader MyCompute;
	ComputeBuffer VertexBuffer;
	ComputeBuffer CenterBuffer;
	Vert[] vertex;			// les vertex du mesh
	List<Cent> centerZone; 	//la list a incrémenter de center, donc nombre de zone
	float MinDist = 0.05f; 		// la distance minimum a laquelle doivent etre 2 centre

	void Start () {


		centerZone = new List<Cent>();

		// on recherche le mesh du gameobject et assign se vertices a vertex
		Mesh mf = GetComponent<MeshFilter>().mesh;
		Vector3[] vertice = mf.vertices;
		Color[] Col = new Color[vertice.Length];

		vertex = new Vert[vertice.Length];

		for ( int i = 0 ; i < vertice.Length ; i ++){
			vertex[i].pos = vertice[i];
			vertex[i].Couleur = new Color(1,1,1,1);
		}
			
		for ( int c = 0 ; c < 3500 ; c++) {
			FindCenter();
		}


		float TP = Time.realtimeSinceStartup;

		for ( int i = 0; i < 1000 ; i++){
			UseShader();
		}

		for ( int i = 0 ; i < vertice.Length ; i ++){
			Col[i] = vertex[i].Couleur;

		}
		mf.colors = Col;


		//print("zoneCount : " + centerZone.Count);	
		print(" temps GPU : " + ((Time.realtimeSinceStartup - TP)/1000.0f) );

		
	}

	void UseShader(){

		int IndexKernel = MyCompute.FindKernel("CSMain");
		VertexBuffer = new ComputeBuffer(vertex.Length,28);
		VertexBuffer.SetData(vertex);
		CenterBuffer = new ComputeBuffer(centerZone.Count,28);
		CenterBuffer.SetData(centerZone.ToArray() );
		int NbThread = Mathf.NextPowerOfTwo(vertex.Length/1024 +1);

		MyCompute.SetBuffer( IndexKernel, "Vertex",VertexBuffer);
		MyCompute.SetBuffer( IndexKernel, "Center",CenterBuffer);
		MyCompute.Dispatch(IndexKernel,NbThread,1,1);
		VertexBuffer.GetData(vertex);

		VertexBuffer.Release();
		CenterBuffer.Release(); 

	}


	void FindCenter(){

		Vector3 CenterToAdd =  vertex[Random.Range(0,vertex.Length-1)].pos;

		for ( int i = 0 ; i < centerZone.Count ; i++){
			float dist = (CenterToAdd-centerZone[i].pos).magnitude;
			if( dist < MinDist){				
				return;
			}
		}

		Color Col = new Color(Random.value, Random.value,Random.value);
		centerZone.Add(new Cent(CenterToAdd, Col));
	}
}
