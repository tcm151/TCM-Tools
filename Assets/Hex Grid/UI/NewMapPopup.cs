using TMPro;
using UnityEngine;


namespace TCM.HexGrid.UI
{
    public class NewMap : MonoBehaviour
    {
        public HexGrid hexGrid;
        public TMP_InputField xInput, zInput;

        public void Open() => gameObject.SetActive(true);
        public void Close() => gameObject.SetActive(false);

        private void CreateMap(int x, int z)
        {
            hexGrid.CreateMap(x, z);
            Close();
        }

        public void CreateSmallMap() => CreateMap(20, 15);
        public void CreateMediumMap() => CreateMap(40, 30);
        public void CreateLargeMap() => CreateMap(80, 60);
        public void CreateExtraLargeMap() => CreateMap(160, 120);

        public void CreateCustomMap()
        {
            if (xInput.text.Length == 0 || zInput.text.Length == 0) return;
            CreateMap(int.Parse(xInput.text), int.Parse(zInput.text));
            // Debug.Log(int.Parse(xInput.text));
            // Debug.Log(int.Parse(zInput.text));
        }
    }
}