using UnityEngine;
using UnityEngine.UI;

namespace Starflare.Bowling
{
    /// <summary>What a <see cref="LaneSurface"/> covers.</summary>
    public enum LaneSurfaceKind { Masking, Approach, Gutters, Boards }

    /// <summary>
    /// One textured surface of the lanes, built as geometry through the lane's projection: the
    /// approach, the gutters, the boards (your lane and three either side, dimmer the further
    /// out), or the masking panel over the far end. Cut into rows down the lane so the texture
    /// keeps its perspective. Lit by vertex colour: the house lights on the approach, dimmer
    /// down the lane, bright on the deck.
    /// </summary>
    public sealed class LaneSurface : MaskableGraphic
    {
        LaneSurfaceKind _kind;
        Texture _texture;
        LaneView _lane;

        public override Texture mainTexture => _texture != null ? _texture : s_WhiteTexture;

        /// <summary>Draws this surface with this texture, placed by the lane's frame.</summary>
        public void Show(LaneSurfaceKind kind, Texture texture, LaneView lane)
        {
            _kind = kind;
            _texture = texture;
            _lane = lane;
            SetMaterialDirty();
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            var r = rectTransform.rect;
            if (r.width <= 0 || r.height <= 0) return;
            var view = _lane != null ? _lane.ProjectionOver(r) : new LaneProjection(r, r);
            switch (_kind)
            {
                case LaneSurfaceKind.Masking: Masking(vh, view); break;
                case LaneSurfaceKind.Approach: Approach(vh, view); break;
                case LaneSurfaceKind.Gutters: Gutters(vh, view); break;
                case LaneSurfaceKind.Boards: Boards(vh, view); break;
            }
        }

        void Boards(VertexHelper vh, LaneProjection view)
        {
            double half = LaneGeometry.HalfWidth;
            var rows = LaneProjection.Rows(0, LaneGeometry.PitFt);
            for (int lane = -LaneProjection.Neighbours; lane <= LaneProjection.Neighbours; lane++)
            {
                double centre = lane * LaneProjection.Pitch;
                Strip(vh, view, rows, centre - half, centre + half, lane,
                    x => (x - centre + half) / (2 * half), ft => ft / LaneArt.BoardsTileFt);
            }
        }

        void Gutters(VertexHelper vh, LaneProjection view)
        {
            double inner = LaneGeometry.HalfWidth, outer = LaneGeometry.KickbackX, width = outer - inner;
            var rows = LaneProjection.Rows(0, LaneGeometry.PitFt);
            for (int lane = -LaneProjection.Neighbours; lane <= LaneProjection.Neighbours; lane++)
            {
                double centre = lane * LaneProjection.Pitch;
                // The picture runs from the lane's edge outward, so the left gutter is its mirror.
                Strip(vh, view, rows, centre + inner, centre + outer, lane, x => (x - centre - inner) / width, ft => ft / LaneArt.GutterTileFt);
                Strip(vh, view, rows, centre - outer, centre - inner, lane, x => (centre - inner - x) / width, ft => ft / LaneArt.GutterTileFt);
            }
        }

        void Approach(VertexHelper vh, LaneProjection view)
        {
            var rows = LaneProjection.Rows(view.BottomFt, 0, 0.5);
            for (int lane = -LaneProjection.Neighbours; lane <= LaneProjection.Neighbours; lane++)
            {
                double centre = lane * LaneProjection.Pitch;
                Strip(vh, view, rows, centre - LaneProjection.Pitch / 2, centre + LaneProjection.Pitch / 2, lane,
                    x => x / LaneArt.ApproachTileM, ft => ft / LaneArt.ApproachTileFt);
            }
        }

        /// <summary>Across the top above the pit, tiled sideways at the texture's own shape, lit from below.</summary>
        void Masking(VertexHelper vh, LaneProjection view)
        {
            var r = view.Rect;
            float bottom = view.MaskingY, top = Mathf.Min(r.yMax, view.MaskingTopY), height = Mathf.Max(1, top - bottom);
            float aspect = _texture != null && _texture.height > 0 ? _texture.width / (float)_texture.height : 1.5f;
            float tile = height * aspect;
            float u0 = (r.xMin - r.center.x) / tile + 0.5f, u1 = (r.xMax - r.center.x) / tile + 0.5f;
            Color low = color, high = color * 0.8f;
            low.a = high.a = color.a;
            int i = vh.currentVertCount;
            vh.AddVert(new Vector2(r.xMin, bottom), low, new Vector2(u0, 0));
            vh.AddVert(new Vector2(r.xMin, top), high, new Vector2(u0, 1));
            vh.AddVert(new Vector2(r.xMax, top), high, new Vector2(u1, 1));
            vh.AddVert(new Vector2(r.xMax, bottom), low, new Vector2(u1, 0));
            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i, i + 2, i + 3);
        }

        /// <summary>A band from one x to another (metres) down the given rows, a quad a row.</summary>
        void Strip(VertexHelper vh, LaneProjection view, double[] rows, double fromX, double toX, int lane,
            System.Func<double, double> u, System.Func<double, double> v)
        {
            float dim = LaneProjection.Dim(lane);
            for (int k = 0; k + 1 < rows.Length; k++)
            {
                double near = rows[k], far = rows[k + 1];
                var nearColour = Lit(near, dim);
                var farColour = Lit(far, dim);
                int i = vh.currentVertCount;
                vh.AddVert(view.Project(fromX, near), nearColour, new Vector2((float)u(fromX), (float)v(near)));
                vh.AddVert(view.Project(fromX, far), farColour, new Vector2((float)u(fromX), (float)v(far)));
                vh.AddVert(view.Project(toX, far), farColour, new Vector2((float)u(toX), (float)v(far)));
                vh.AddVert(view.Project(toX, near), nearColour, new Vector2((float)u(toX), (float)v(near)));
                vh.AddTriangle(i, i + 1, i + 2);
                vh.AddTriangle(i, i + 2, i + 3);
            }
        }

        Color Lit(double ft, float dim)
        {
            float light = LaneProjection.Light(ft) * dim;
            return new Color(color.r * light, color.g * light, color.b * light, color.a);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }
    }
}
