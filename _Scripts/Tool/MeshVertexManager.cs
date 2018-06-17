using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGame.CreateMesh
{
    /// <summary>
    /// mesh 顶点管理 
    /// 功能：给添加的物体增加顶点可视化编辑功能                by: heyang 2017/10/09
    /// </summary>
    [ExecuteInEditMode]
    public class MeshVertexManager : MonoBehaviour
    {
        /* 操作 */
        private Vector3[] _vertexs;         //顶点数组    
        private Mesh _mesh;                 //可操作mesh
        private MeshFilter _meshFilter;
        private Transform _ctrlTrans;       //顶点控制器的父节点

        /* 备份 还原 */
        private Vector3[] _copyVertexs;

        private void Awake()
        {
            _meshFilter = transform.GetComponent<MeshFilter>();      
        }

        private void OnDisable()
        {
            //删除组件的时候把创建的mesh控制器给销毁掉
            if (_ctrlTrans != null)
            {
                DestroyImmediate(_ctrlTrans.gameObject);
            }
        }

        /// <summary>
        /// 创建各个顶点的控制器
        /// </summary>
        public void CreateVertex()
        {
            if (_meshFilter == null)
            {
                _meshFilter = transform.GetComponent<MeshFilter>();
                if (_meshFilter == null)
                {
                    return;
                }
            }
            _mesh = _meshFilter.mesh;

            //顶点备份
            _copyVertexs = _mesh.vertices;

            //创建可操作顶点数组，并替换mesh的引用
            _vertexs = new Vector3[_mesh.vertices.Length];
            _mesh.vertices.CopyTo(_vertexs, 0);
            _mesh.vertices = _vertexs;

            GameObject ctrlGroup = new GameObject();
            ctrlGroup.transform.parent = this.transform;
            ctrlGroup.transform.localPosition = Vector3.zero;
            ctrlGroup.name = "vertexRoot";
            _ctrlTrans = ctrlGroup.transform;

            for (int i = 0; i < _vertexs.Length; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = ctrlGroup.transform;
                cube.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                cube.transform.localPosition = _vertexs[i];
                cube.name = i.ToString();
                DestroyImmediate(cube.GetComponent<BoxCollider>());
                MeshVertex meshvertex = cube.AddComponent<MeshVertex>();
                meshvertex.SetMeshVertexData(this, i);
            }
        }

        /// <summary>
        /// 结束编辑，还原mesh
        /// </summary>
        public void EndEditor()
        {
            if (_ctrlTrans != null)
            {
                DestroyImmediate(_ctrlTrans.gameObject);
            }

            //还原mesh
            _mesh.vertices = _copyVertexs;
        }

        /// <summary>
        /// 完成编辑，改变mesh
        /// </summary>
        public void FinishEditor()
        {
            if (_ctrlTrans != null)
            {
                DestroyImmediate(_ctrlTrans.gameObject);
            }
        }
        
        /// <summary>
        /// 更新对应index的坐标信息
        /// </summary>
        public void UpdateVertex(int index, Vector3 position)
        {
            if (index >= 0 && index < _vertexs.Length)
            {
                _vertexs[index] = position;
                _mesh.vertices = _vertexs;
            }
        }
    }
}

