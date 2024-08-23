//UNITY_SHADER_NO_UPGRADE
#ifndef MYHLSLINCLUDE_INCLUDED
#define MYHLSLINCLUDE_INCLUDED

half3 doTransform(half4x4 transform, half3 pt)
{
    return mul(transform, half4(pt.x, pt.y, pt.z, 1)).xyz;
}

half4 loadBoneMatrixTexture(Texture2D<float4> animatedBoneMatrices, int frameIndex, int boneIndex, int i)
{
    return animatedBoneMatrices.Load(int3((boneIndex * 3) + i, frameIndex, 0));
}

half4x4 extractRotationMatrix(half4x4 m)
{
    half sx = length(half3(m[0][0], m[0][1], m[0][2]));
    half sy = length(half3(m[1][0], m[1][1], m[1][2])); 
    half sz = length(half3(m[2][0], m[2][1], m[2][2]));

    // if determine is negative, we need to invert one scale
    half det = determinant(m);
    if (det < 0) {
        sx = -sx;
    }

    half invSX = 1.0 / sx;
    half invSY = 1.0 / sy;
    half invSZ = 1.0 / sz;

    m[0][0] *= invSX;
    m[0][1] *= invSX;
    m[0][2] *= invSX;
    m[0][3] = 0;

    m[1][0] *= invSY;
    m[1][1] *= invSY;
    m[1][2] *= invSY;
    m[1][3] = 0;

    m[2][0] *= invSZ;
    m[2][1] *= invSZ;
    m[2][2] *= invSZ;
    m[2][3] = 0;

    m[3][0] = 0;
    m[3][1] = 0;
    m[3][2] = 0;
    m[3][3] = 1;

    return m;
}

void calculateFrameValues(half3 position, half3 normal, half3 tangent,
    Texture2D<float4> animatedBoneMatrices, 
    half2 boneWeights[6], int frameIndex, 
    out half3 positionOut, out half3 normalOut, out half3 tangentOut)
{
    positionOut = normalOut = tangentOut = half3(0, 0, 0);
    for(int i = 0; i < 6; i++)
    {
        half boneWeight = boneWeights[i].y;
        if(boneWeight != 0)
        {
            half boneIndex = boneWeights[i].x;

            half4 m0 = loadBoneMatrixTexture(animatedBoneMatrices, frameIndex, boneIndex, 0); 
            half4 m1 = loadBoneMatrixTexture(animatedBoneMatrices, frameIndex, boneIndex, 1); 
            half4 m2 = loadBoneMatrixTexture(animatedBoneMatrices, frameIndex, boneIndex, 2); 
            half4 m3 = half4(0,0,0,1);
            
            half4x4 animatedBoneMatrix = half4x4(m0, m1, m2, m3);
            half4x4 rotationMatrix = extractRotationMatrix(animatedBoneMatrix);

            positionOut += boneWeight * doTransform(animatedBoneMatrix, position);
            
            normalOut += boneWeight * doTransform(rotationMatrix, normal);
            tangentOut += boneWeight * doTransform(rotationMatrix, tangent);
        }
    }
}

void AnimateBlend_half(half3 position, half3 normal, half3 tangent, 
    half3x4 uvs, Texture2D<float4> animatedBoneMatrices, half4x3 animationState,
    out half3 positionOut, out half3 normalOut, out half3 tangentOut) 
{
    positionOut = half3(0, 0, 0);
    normalOut = half3(0, 0, 0);
    tangentOut = half3(0, 0, 0);
    half2 boneWeights[6] = {
        half2(uvs._m00, uvs._m01),  half2(uvs._m02, uvs._m03),
        half2(uvs._m10, uvs._m11),  half2(uvs._m12, uvs._m13),
        half2(uvs._m20, uvs._m21),  half2(uvs._m22, uvs._m23) };
    
    for(int blendIndex = 0; blendIndex < 4; blendIndex++)
    {
        half blendFactor = animationState[blendIndex][0];  
        if(blendFactor > 0)
        {
            half transitionNextFrame = animationState[blendIndex][1];
            half prevFrameFrac = 1.0 - transitionNextFrame;
            half frameIndex = animationState[blendIndex][2];
            half3 posOutBefore, posOutAfter, normalOutBefore, normalOutAfter, tangentOutBefore, tangentOutAfter;
            calculateFrameValues(position, normal, tangent, animatedBoneMatrices, boneWeights, frameIndex, 
                posOutBefore, normalOutBefore, tangentOutBefore);
            calculateFrameValues(position, normal, tangent, animatedBoneMatrices, boneWeights, frameIndex + 1,
                posOutAfter, normalOutAfter, tangentOutAfter);
            positionOut += blendFactor * (prevFrameFrac * posOutBefore + transitionNextFrame * posOutAfter);
            normalOut += blendFactor * (prevFrameFrac * normalOutBefore + transitionNextFrame * normalOutAfter);
            tangentOut += blendFactor * (prevFrameFrac * tangentOutBefore + transitionNextFrame * tangentOutAfter);
        }        
    }
}
#endif //MYHLSLINCLUDE_INCLUDED