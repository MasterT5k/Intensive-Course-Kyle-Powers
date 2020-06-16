using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevHQ.FileBase.Missile_Launcher.Missile;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Tower.TowerPlacementNS;

/*
 *@author GameDevHQ 
 * For support, visit gamedevhq.com
 */

namespace GameDevHQ.FileBase.Missile_Launcher
{
    public class Missile_Launcher : MonoBehaviour, ITower
    {
        public enum MissileType
        {
            Normal,
            Homing
        }

        [SerializeField]
        private GameObject _missilePrefab = null; //holds the missle gameobject to clone
        [SerializeField]
        private MissileType _missileType = MissileType.Normal; //type of missle to be launched
        [SerializeField]
        private GameObject[] _misslePositions = null; //array to hold the rocket positions on the turret
        [SerializeField]
        private float _fireDelay = 0f; //fire delay between rockets
        [SerializeField]
        private float _launchSpeed = 0f; //initial launch speed of the rocket
        [SerializeField]
        private float _power = 0f; //power to apply to the force of the rocket
        [SerializeField]
        private float _fuseDelay = 0f; //fuse delay before the rocket launches
        [SerializeField]
        private float _reloadTime = 0f; //time in between reloading the rockets
        [SerializeField]
        private float _destroyTime = 10.0f; //how long till the rockets get cleaned up
        private bool _launched; //bool to check if we launched the rockets
        [SerializeField]
        private Transform _target = null; //Who should the rocket fire at?
        [SerializeField]
        private int _warFundValue = 0;

        public int WarFundValue { get; set; } = 500;
        public int TowerID { get; set; } = 1;
        public MeshRenderer AttackRange { get; set; }
        public GameObject EnemyToTarget { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsEnemyInRange { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        private void OnEnable()
        {
            TowerPlacement.onSelectTower += PlaceMode;
        }

        private void OnDisable()
        {
            TowerPlacement.onSelectTower -= PlaceMode;
        }

        private void Start()
        {
            AttackRange = transform.Find("Attack Range").GetComponent<MeshRenderer>();
            if (AttackRange != null)
            {
                AttackRange.enabled = false;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _launched == false) //check for space key and if we launched the rockets
            {
                _launched = true; //set the launch bool to true
                StartCoroutine(FireRocketsRoutine()); //start a coroutine that fires the rockets. 
            }
        }

        IEnumerator FireRocketsRoutine()
        {
            for (int i = 0; i < _misslePositions.Length; i++) //for loop to iterate through each missle position
            {
                GameObject rocket = Instantiate(_missilePrefab) as GameObject; //instantiate a rocket

                rocket.transform.parent = _misslePositions[i].transform; //set the rockets parent to the missle launch position 
                rocket.transform.localPosition = Vector3.zero; //set the rocket position values to zero
                rocket.transform.localEulerAngles = new Vector3(-90, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
                rocket.transform.parent = null; //set the rocket parent to null

                rocket.GetComponent<GameDevHQ.FileBase.Missile_Launcher.Missile.Missile>().AssignMissleRules(_missileType, _target, _launchSpeed, _power, _fuseDelay, _destroyTime); //assign missle properties 

                _misslePositions[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired

                yield return new WaitForSeconds(_fireDelay); //wait for the firedelay
            }

            for (int i = 0; i < _misslePositions.Length; i++) //itterate through missle positions
            {
                yield return new WaitForSeconds(_reloadTime); //wait for reload time
                _misslePositions[i].SetActive(true); //enable fake rocket to show ready to fire
            }

            _launched = false; //set launch bool to false
        }

        public void PlaceMode(bool inPlaceMode)
        {
            if (inPlaceMode == true)
            {
                AttackRange.enabled = true;
            }
            else
            {
                AttackRange.enabled = false;
            }
        }

        public void AttackEnemy(GameObject enemy)
        {
            throw new System.NotImplementedException();
        }

        public void NoEnemiesInRange()
        {
            throw new System.NotImplementedException();
        }
    }
}

