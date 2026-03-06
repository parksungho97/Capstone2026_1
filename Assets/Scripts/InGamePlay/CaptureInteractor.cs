using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Network
{
    public class CaptureInteractor : NetworkBehaviour
    {
        private readonly List<CaptureZoneController> mZones = new List<CaptureZoneController>();

        private bool mIsHolding = false;

        private CaptureZoneController GetCurrentZone()
        {
            if (mZones.Count == 0)
                return null;

            return mZones[mZones.Count - 1];
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            CaptureZoneController zone = GetCurrentZone();

            if (Input.GetKeyDown(KeyCode.O))
            {
                mIsHolding = true;
                Debug.Log("O key pressed");
                if (zone != null)
                    zone.StartCaptureServerRpc();
            }

            if (Input.GetKeyUp(KeyCode.O))
            {
                if (zone != null)
                    zone.StopCaptureServerRpc();

                mIsHolding = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsOwner)
                return;

            CaptureZoneController zone = other.GetComponent<CaptureZoneController>();
            if (zone == null)
                zone = other.GetComponentInParent<CaptureZoneController>();

            if (zone == null)
                return;

            if (!mZones.Contains(zone))
                mZones.Add(zone);

            if (mIsHolding)
                zone.StartCaptureServerRpc();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsOwner)
                return;

            CaptureZoneController zone = other.GetComponent<CaptureZoneController>();
            if (zone == null)
                zone = other.GetComponentInParent<CaptureZoneController>();

            if (zone == null)
                return;

            bool wasCurrentZone = (GetCurrentZone() == zone);

            if (wasCurrentZone && mIsHolding)
                zone.StopCaptureServerRpc();

            mZones.Remove(zone);

            if (mIsHolding)
            {
                CaptureZoneController currentZone = GetCurrentZone();
                if (currentZone != null)
                    currentZone.StartCaptureServerRpc();
            }
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;

            CaptureZoneController zone = GetCurrentZone();
            if (mIsHolding && zone != null)
                zone.StopCaptureServerRpc();

            mIsHolding = false;
            mZones.Clear();
        }
    }
}