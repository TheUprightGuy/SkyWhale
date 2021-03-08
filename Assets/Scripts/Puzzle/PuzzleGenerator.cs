using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Puzzle
{
    public class PuzzleGenerator : MonoBehaviour
    {
        public int _currentType;
        public int puzzleSize = 4;//num of tiles in each row and col
        public float distanceFromStartingTiles = 5f;
        public List<List<GameObject>> startingTiles;
        public List<GameObject> tileMeshes;
        public List<GameObject> tilePrefabs;
        public List<List<GameObject>> tiles;
        public float tileSpacing = 1.1f;
        public bool disabled;

        public Vector3 RestartPos = Vector3.zero;
        public void Awake()
        {
            tiles = new List<List<GameObject>>();
            startingTiles = new List<List<GameObject>>();
            var tilePos = gameObject.transform.position;
            var right = transform.right;
            /*tilePos += right * ((float) puzzleSize / 2 + 0.5f);
            tilePos = GenerateStartingTiles(tilePos);
            tilePos -= right * ((float) puzzleSize / 2 + 0.5f);*/
            tilePos += transform.forward * distanceFromStartingTiles;
            GenerateTiles(tilePos);
            disabled = false;
            GeneratePath();
            _currentType = 0;
            RestartPos += transform.position;
        }

        private void GeneratePath()
        {
            //Choose a random starting tile in the first row
            var rand = Random.Range(0, puzzleSize);
            var currentTile = new Vector2(0, rand);
            SetPathTile(currentTile);
            //For each row, choose a random col which will serve as a target tile
            for (var i = 0; i < puzzleSize; i++)
            {
                rand = Random.Range(0, puzzleSize);
                var targetTile = new Vector2(i, rand);
                //If current tile col number is greater than target til col
                //keep moving left until you reach the same tile, altering the tile type as you go
                while (currentTile.y > targetTile.y)
                {
                    currentTile.y -= 1;
                    SetPathTile(currentTile);
                }

                while (currentTile.y < targetTile.y)
                {
                    currentTile.y += 1;
                    SetPathTile(currentTile);
                }

                currentTile.x += 1;
                if (currentTile.x < puzzleSize) SetPathTile(currentTile);
            }
        }

        private void SetPathTile(Vector2 tileCoordinate)
        {
            var tileRow = tiles[(int) tileCoordinate.y];
            var tile = tileRow[(int) tileCoordinate.x];
            var tileSO = tile.GetComponent<PuzzleTile>().puzzleTileSo;
            tile.GetComponent<PuzzleTile>().puzzleTileSo.type = _currentType;
            Destroy(tile.transform.GetChild(0).gameObject);
            Instantiate(tileMeshes[_currentType], tile.transform);
            tile.name = tilePrefabs[_currentType].name;
            //Debug.Log("Col: " + tileSO.col + " Row: " + tileSO.row + " Type: " + tile.name);
            _currentType += 1;
            _currentType %= tilePrefabs.Count;
        }

        private void GenerateTiles(Vector3 tilePos)
        {
            for (var i = 0; i < puzzleSize; i++)
            {
                var newRow = new List<GameObject>();
                for (var j = 0; j < puzzleSize; j++)
                {
                    var randTile = Random.Range(0, tilePrefabs.Count);
                    var newTile = Instantiate(tilePrefabs[randTile], tilePos, transform.rotation, transform);
                    Instantiate(tileMeshes[randTile], newTile.transform);
                    newTile.name = tilePrefabs[randTile].name;
                    var ptSo = ScriptableObject.CreateInstance<PuzzleTileSO>();
                    ptSo.Initialise(randTile, j, i);
                    var puzzleTile = newTile.GetComponent<PuzzleTile>();
                    puzzleTile.puzzleTileSo = ptSo;
                    puzzleTile.puzzleGenerator = this;
                    newRow.Add(newTile);
                    tilePos += transform.forward * tileSpacing;
                }

                tiles.Add(newRow);
                tilePos -= transform.forward * puzzleSize * tileSpacing;
                tilePos += transform.right * tileSpacing;
            }
        }

        private Vector3 GenerateStartingTiles(Vector3 tilePos)
        {
            var currentTile = 0;
            var newRow = new List<GameObject>();
            for (var j = 0; j < tilePrefabs.Count; j++)
            {
                var newTile = Instantiate(tilePrefabs[currentTile], tilePos, transform.rotation, transform);
                newTile.name = tilePrefabs[currentTile].name;
                var ptSo = ScriptableObject.CreateInstance<PuzzleTileSO>();
                ptSo.Initialise(currentTile, j, 0);
                var puzzleTile = newTile.GetComponent<PuzzleTile>();
                puzzleTile.puzzleTileSo = ptSo;
                puzzleTile.puzzleGenerator = this;

                newRow.Add(newTile);

                tilePos += transform.forward * tileSpacing;
                currentTile += 1;
            }

            startingTiles.Add(newRow);
            return tilePos;
        }

        public bool CheckIfCorrectType(int tileType)
        {
            if (tileType == _currentType)
            {
                _currentType += 1;
                _currentType %= tilePrefabs.Count;
                return true;
            }

            _currentType = 0;
            return false;
        }

        public void ResetPuzzle(GameObject player)
        {
            if (disabled) return;
            ResetTiles();
            TeleportToStart(player);
        }

        private void ResetTiles()
        {
            foreach (var tile in tiles.SelectMany(row => row))
                tile.GetComponent<PuzzleTile>().puzzleTileSo.triggered = false;

            foreach (var tile in startingTiles.SelectMany(row => row))
                tile.GetComponent<PuzzleTile>().puzzleTileSo.triggered = false;
        }

        private void TeleportToStart(GameObject player)
        {
            player.transform.position = RestartPos; //transform.position + Vector3.up * player.transform.position.y + Vector3.back;
        }

        public void FoundCollectable()
        {
            disabled = true;
            cam.m_Priority = 0;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(RestartPos, 1.0f);
        }

        public Cinemachine.CinemachineVirtualCamera cam;
        private void OnTriggerEnter(Collider other)
        {
            TestMovement player = other.GetComponent<TestMovement>();
            if (player && !disabled)
            {
                CallbackHandler.instance.LerpCam();
                cam.m_Priority = 20;
                // switch to fixed cam;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            TestMovement player = other.GetComponent<TestMovement>();
            if (player)
            {
                cam.m_Priority = 0;
                // switch to normal cam;
            }
        }
    }
}