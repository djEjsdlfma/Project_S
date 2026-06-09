using System;
using LSW._02._Code.Player;
using MoonLib.ScriptFinder_Pro.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Moon._01.Script.Gimmick
{
    public class ResetAndIsTeleport : MonoBehaviour
    {
        [SerializeField] private bool teleport = false;
        [SerializeField] private Transform teleportTarget;
        [SerializeField] private ScriptAllFinderSO jumpAndSlowedFinder;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                jumpAndSlowedFinder.GetTarget<JumpAndSlowed>()?.ForEach(j => j.Used = false);
                if (teleport)
                {
                    other.transform.position = teleportTarget.position;
                }

                other.GetComponent<Player>().ResetSpeeds();
            }
        }
    }
}