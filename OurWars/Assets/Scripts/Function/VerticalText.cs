using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Extension/VerticalText")]
[RequireComponent(typeof(UnityEngine.UI.Text))]
public class VerticalText : BaseMeshEffect
{
    Vector2 SetOneCharPosition(ref VertexHelper vh, int idx, Vector3 pos0)
    {
        if (idx % 4 != 0 || vh == null) return Vector2.zero;

        var tex0 = new UIVertex();
        vh.PopulateUIVertex(ref tex0, idx);

        var tex1 = new UIVertex();
        vh.PopulateUIVertex(ref tex1, idx + 1);

        var tex2 = new UIVertex();
        vh.PopulateUIVertex(ref tex2, idx + 2);

        var tex3 = new UIVertex();
        vh.PopulateUIVertex(ref tex3, idx + 3);

        float width = tex0.position.x - tex1.position.x;
        float height = tex0.position.y - tex3.position.y;
        var offset = pos0 - tex0.position;

        tex0.position = pos0;
        tex1.position += offset;
        tex2.position += offset;
        tex3.position += offset;

        vh.SetUIVertex(tex0, idx);
        vh.SetUIVertex(tex1, idx + 1);
        vh.SetUIVertex(tex2, idx + 2);
        vh.SetUIVertex(tex3, idx + 3);
        //  Debug.LogError(offset + "      " + tex0.uv0 + "   " + tex0.uv1);

        return new Vector2(width, height);
    }
    public override void ModifyMesh(VertexHelper vh)
    {
        //  var num = vh.currentVertCount;
        var text = this.GetComponent<Text>();
        var strs = text.text.Split('|');
        int ColStartIndex = 0;
        int vertexStartIndex = 0;
        /*   for (int colIndex = 0; colIndex < strs.Length; colIndex++, vertexStartIndex += 4)
            {
                {
                    UIVertex VertexOne = new UIVertex();
                    vh.PopulateUIVertex(ref VertexOne, 0);
                    SetOneCharPosition(vh, 0, VertexOne.position);
                }
            }
            */
        UIVertex VertexOne0 = new UIVertex();
        vh.PopulateUIVertex(ref VertexOne0, 0);


        float HEIGHT_PER_UNIT = 30f;
        float height_current = 0f;
        for (int col = 0; col < vh.currentVertCount; col++)
        {
            UIVertex VertexOne = new UIVertex();
            vh.PopulateUIVertex(ref VertexOne, col);
            height_current = 0f;
            float dx = 0f;
            dx = col * 30f;

            for (int i = 0; i < vh.currentVertCount; i += 4)
            {
                float h = this.SetOneCharPosition(ref vh, i, VertexOne0.position + new Vector3(dx, 0f - HEIGHT_PER_UNIT * ((i) / 4), 0f)).y;
                height_current += 30f;
                if (height_current + 30f >= this.GetComponent<RectTransform>().rect.height)
                    return;
                //  Debug.LogError(this.GetComponent<RectTransform>().rect.height + "   cur=" + height_current);
            }
        }
    }

}
