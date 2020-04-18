﻿/* Written by Avi Vajpeyi
 */


using UnityEngine;
using System.Collections;

public class AircraftExplosion : MonoBehaviour
{
    private AudioClip _explosionAudio;
    private GameObject[] _renderedGameObjects;
    private AudioSource _audioSource;


    public void SetReferences(AudioClip explosionAudio, GameObject[] renderedGameObjects)
    {
        _explosionAudio = explosionAudio;
        _renderedGameObjects = renderedGameObjects;
    }


    public void Explode()
    {
        SplitMeshes();
        PlayExplosionSound();
        Destroy(this.gameObject);
    }

    void Start()
    {
        _audioSource = FindObjectOfType<AudioSource>();
    }

    IEnumerator DestroyDebrisCoroutine(GameObject GO)
    {
        yield return new WaitForSeconds(2 + Random.Range(0.0f, 5.0f));
        GO.GetComponent<MeshFilter>().sharedMesh.Clear();
        Destroy(GO);
    }


    void PlayExplosionSound()
    {
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(_explosionAudio, 0.7F);
            Destroy(this, _audioSource.time + 0.5f);
        }
    }

    void SplitMeshes()
    {
        foreach (GameObject g in _renderedGameObjects)
        {
            SplitMesh(
                mf: g.GetComponent<MeshFilter>(),
                mr: g.GetComponent<MeshRenderer>()
            );
        }
    }

    void SplitMesh(MeshFilter mf, MeshRenderer mr)
    {
        Mesh M = mf.mesh;
        Vector3[] verts = M.vertices;
        Vector3[] normals = M.normals;
        Vector2[] uvs = M.uv;
        for (int submesh = 0; submesh < M.subMeshCount; submesh++)
        {
            int[] indices = M.GetTriangles(submesh);
            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3[] newVerts = new Vector3[3];
                Vector3[] newNormals = new Vector3[3];
                Vector2[] newUvs = new Vector2[3];
                for (int n = 0; n < 3; n++)
                {
                    int index = indices[i + n];
                    newVerts[n] = verts[index];
                    newUvs[n] = uvs[index];
                    newNormals[n] = normals[index];
                }

                Mesh mesh = new Mesh();
                mesh.vertices = newVerts;
                mesh.normals = newNormals;
                mesh.uv = newUvs;

                mesh.triangles = new int[] {0, 1, 2, 2, 1, 0};

                GameObject GO = new GameObject("Triangle " + (i / 3));
                GO.transform.position = transform.position;
                GO.transform.rotation = transform.rotation;
                GO.AddComponent<MeshRenderer>().material = mr.materials[submesh];
                GO.AddComponent<MeshFilter>().mesh = mesh;
                GO.AddComponent<BoxCollider>();
                GO.AddComponent<Rigidbody>()
                    .AddExplosionForce(20, transform.position, 30);

                StartCoroutine(DestroyDebrisCoroutine(GO));
            }
        }

        mr.enabled = false;
    }
}