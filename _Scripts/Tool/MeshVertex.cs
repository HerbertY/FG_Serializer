using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGame.CreateMesh
{
    /// <summary>
    /// mesh 顶点控制器
    /// </summary>
    [ExecuteInEditMode]
    public class MeshVertex : MonoBehaviour
    {
        //父对象上的管理器
        public MeshVertexManager manager;
        //对应的顶点序号
        public int Index { get; set; }

        private void Update()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                //返回自身坐标信息
                if (manager != null)
                {
                    manager.UpdateVertex(Index, transform.localPosition);
                }
            }
        }

        /// <summary>
        /// 设置顶点控制器的数据
        /// </summary>
        /// <param name="index">顶点序号</param>
        public void SetMeshVertexData(MeshVertexManager mgr, int index)
        {
            Index = index;
            manager = mgr;
        }
    }
}


