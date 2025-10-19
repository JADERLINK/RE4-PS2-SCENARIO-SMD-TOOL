﻿using SHARED_PS2_BIN.ALL;
using SHARED_PS2_BIN.EXTRACT;
using System;
using System.Collections.Generic;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT
{
    public class PS2GenericModelBIN
    {
        public (float vx, float vy, float vz)[] Vertex_Position_Array; // ja escalado
        public (float nx, float ny, float nz)[] Vertex_Normal_Array; // ja normalizado, vai ser tamanho 0 se não tiver.
        public (float tu, float tv)[] Vertex_UV_Array; // so colocar no obj
        public (float a, float r, float g, float b)[] Vertex_Color_Array; // vai ser tamanho 0 se não tiver.

        public GenericMaterialBIN[] Materials; // onde esta o material e as faces;

        public BinType binType = BinType.Default;
    }

    public class GenericMaterialBIN
    {
        public MaterialPart material;

        public (GenericFaceIndex i1, GenericFaceIndex i2, GenericFaceIndex i3)[] face_index_array;
    }

    public struct GenericFaceIndex
    {
        public int indexVertex;
        public int indexNormal;
        public int indexUV;
    }
}
