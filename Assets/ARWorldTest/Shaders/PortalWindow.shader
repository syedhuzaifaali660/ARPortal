﻿Shader "Huzaifa/World/PortalWindow"
{
    SubShader
    {

		Zwrite off
		ColorMask 0
		Cull off
		Stencil
		{
			Ref 1
			Pass replace
		}


        Pass
        {
        }
    }
}
