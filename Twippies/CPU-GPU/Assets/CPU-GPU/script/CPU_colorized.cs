using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU_colorized : MonoBehaviour {


	Vector3[] vertex;			// les vertex du mesh
	List<Vector3> centerZone; 	//la list a incrémenter de center, donc nombre de zone
	float MinDist = 0.05f; 		// la distance minimum a laquelle doivent etre 2 centre

	void Start () {


		// on initialise la liste des centre
		centerZone = new List<Vector3>();

		// on recherche le mesh du gameobject et assign se vertices a vertex
		Mesh mf = GetComponent<MeshFilter>().mesh;
		vertex = mf.vertices;

		// on fait un certain nombre de recherche de centre
		// il faut beaucoup d'itération pour etre sur que la densité des zone soitent le plus uniforme possible
		for ( int c = 0 ; c < 3500 ; c++) {
			FindCenter();
		}


		// on stock un nouvel array pour la couleur des vertex
		Color[] VertexCol = new Color[vertex.Length];
		// on stock un array de couleur, de même logueur que la liste des centres
		Color[] zoneColor = new Color[centerZone.Count];

		// pour chaque élément de zoneColor, on lui attribu une couleur aléatoire.
		for ( int i = 0 ; i < centerZone.Count ; i++){
			zoneColor[i] = new Color(Random.value, Random.value,Random.value,1);
		}

		float TP = Time.realtimeSinceStartup;
		// pour chacun des vertex du mesh,

		for ( int k = 0 ; k < 5 ; k++){
			for ( int i = 0 ; i < vertex.Length ; i++){
				int temp = 0;
				float distMin = Mathf.Infinity;
				for ( int j = 0 ; j < centerZone.Count ; j ++){				
					float dist = (vertex[i] - centerZone[j]).sqrMagnitude;
					if( dist < distMin){
						temp = j;
						distMin = dist;
					}
				}
				VertexCol[i] = zoneColor[temp];
			}
		}
		// on applique enfin au messh la couleur de tout les vertices.
		mf.colors = VertexCol;

		//print("zoneCount : " + centerZone.Count);
		print(" temps CPU : " + ((Time.realtimeSinceStartup - TP)/5.0f) );


	}

	void FindCenter(){

		// on defini aléatoirement un centre
		Vector3 CenterToAdd =  vertex[Random.Range(0,vertex.Length-1)];

		// on test avec les autre centre deja présent dans la liste
		for ( int i = 0 ; i < centerZone.Count ; i++){
			float dist = (CenterToAdd-centerZone[i]).magnitude;
			// si celui qu'on a choisis est trop proche d'un autre, on s'arrete la pour celui la
			// on passe a l'itération suivante en stoppant la fonction en cours
			if( dist < MinDist){				
				return;
			}
		}

		// si la fonction arrive jusqu'ici, c'est qu'aucun autre centre n'est trop proche
		// on ajoute donc CenterToAdd a la liste. Et on passer a l'itération suivante.
		centerZone.Add(CenterToAdd);

	}

}
