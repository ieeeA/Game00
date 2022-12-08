using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obsolete
{
    public class FireController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _PrefabBullet;

        [SerializeField]
        private float _Speed = 1.0f;

        [SerializeField]
        private int DefaultBulletCount = 30;

        [SerializeField]
        private bool DebugHandleEnable = false;

        private TextHandle _DebugTextHandle;

        [SerializeField]
        private float _ShotgunDispersion = 0.5f;
        [SerializeField]
        private int _ShotgunPhaseAdd = 4;
        [SerializeField]
        private int _PhaseCount = 3;

        public bool IsInfinityBullet { get; set; } = false;

        private int LeftBullet
        {
            get => _LeftBullet;
            set
            {
                _LeftBullet = value;
                if (DebugHandleEnable)
                {
                    _DebugTextHandle.Text = $"Bullet:{_LeftBullet}/{DefaultBulletCount}";
                }
            }
        }
        private int _LeftBullet;

        public void Fire(Vector3 origin, Vector3 dir)
        {
            if (!IsInfinityBullet)
            {
                if (_LeftBullet <= 0)
                {
                    return;
                }
                LeftBullet--;
            }
            //ShootBullet("PShell", origin, dir);
            Shotgun(origin, dir, _ShotgunDispersion, _ShotgunPhaseAdd, _PhaseCount);
        }

        private void ShootBullet(string id, Vector3 origin, Vector3 dir)
        {
            var newBullet = ProjectileManager.Current.Instantiate<Projectile>(id);
            newBullet.transform.position = origin;
            newBullet.transform.LookAt(dir + origin);
            var proj = newBullet.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Configure(_Speed, dir);
            }
        }

        public void Shotgun(Vector3 origin, Vector3 dir, float dispersion, int add, int phaseCount)
        {
            // ここで多分ほんとはVFXのマズルフラッシュ処理みたいなのが入る（試作 of 試作なのでやらないが）
            int bullet = 1;
            for (int i = 0; i < phaseCount; i++)
            {
                for (int j = 0; j < bullet; j++)
                {
                    var orbit2 = MathUtil.Orbit2(j * (Mathf.PI * 2) / bullet) * (dispersion * i);
                    var diff = Quaternion.LookRotation(dir, Vector3.up) * new Vector3(orbit2.x, orbit2.y, 0);
                    var dispDir = dir + diff;
                    ShootBullet("PShell", origin, dispDir);
                }
                bullet += add;
            }
        }

        public bool AddBullet(int bullet)
        {
            if (_LeftBullet < DefaultBulletCount)
            {
                LeftBullet += bullet;
                if (DefaultBulletCount < LeftBullet)
                {
                    LeftBullet = DefaultBulletCount;
                }
                return true;
            }
            return false;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (DebugHandleEnable)
            {
                _DebugTextHandle = StandardTextPlane.Current.CreateTextHandle();
            }
            LeftBullet = DefaultBulletCount;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}