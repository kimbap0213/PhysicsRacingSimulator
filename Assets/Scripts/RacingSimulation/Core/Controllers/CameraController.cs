using UnityEngine;

namespace RacingSimulation.Core.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [System.Serializable]
        public struct CameraPositions
        {
            public Transform camPos;
            public bool canZoom;
            public Vector2 zoomRange;
            public float defaultZoomDistance;
            public bool canOrbit;
            public bool isGlobal;
            [Range(-90, 90)]
            public float verticalOrbitRangeMin;
            [Range(-90, 90)]
            public float verticalOrbitRangeMax;
        }
    
        [SerializeField] private CameraPositions[] cameraPositions;
        [SerializeField] private Transform orbitTarget;
        [SerializeField] private float orbitSpeed;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float moveIntensity;
        private int _currentTarget;
        private Vector2 _currentMousePos;
        private Vector2 _lastMousePos;
        private Vector3 _orbitOriginRotation;
        private bool _newZoomInput;
        private bool _newOrbitInput;

    
        // Start is called before the first frame update
        void Awake()
        {
            _orbitOriginRotation = Vector3.zero;
            // Cursor.visible = false;
            // Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if(_currentTarget >= cameraPositions.Length)
            {
                _currentTarget = 0;
            }

            //Orbiting
            _lastMousePos = _currentMousePos;
            _currentMousePos = _lastMousePos + new Vector2(Input.GetAxis("Mouse X") * moveIntensity, Input.GetAxis("Mouse Y") * moveIntensity);
            float mouseVelX = (_currentMousePos.x - _lastMousePos.x) / Time.deltaTime;
            float mouseVelY = (_currentMousePos.y - _lastMousePos.y) / Time.deltaTime;

            if(cameraPositions[_currentTarget].canOrbit)
            {
                Vector3 futureOrbitOriginRotation = _orbitOriginRotation;
                futureOrbitOriginRotation.y += mouseVelX * orbitSpeed * Time.deltaTime;
                futureOrbitOriginRotation.x -= mouseVelY * orbitSpeed * Time.deltaTime;
                futureOrbitOriginRotation.x = Mathf.Clamp(futureOrbitOriginRotation.x, cameraPositions[_currentTarget].verticalOrbitRangeMin, cameraPositions[_currentTarget].verticalOrbitRangeMax);
            
                _newOrbitInput = true;
                _orbitOriginRotation = futureOrbitOriginRotation;
            }
            if (cameraPositions[_currentTarget].isGlobal)
            {
                cameraPositions[_currentTarget].camPos.rotation = Quaternion.identity;
            }

            //Zooming
            float zoomPos = transform.localPosition.z;
            if(cameraPositions[_currentTarget].canZoom)
            {
                //Zoom Out
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    zoomPos -= zoomSpeed * Time.deltaTime;
                    if(zoomPos <= cameraPositions[_currentTarget].zoomRange.x)
                    {
                        zoomPos = cameraPositions[_currentTarget].zoomRange.x;
                    }
                    _newZoomInput = true;
                }
                //Zoom In
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    zoomPos += zoomSpeed * Time.deltaTime;
                    if(zoomPos >= cameraPositions[_currentTarget].zoomRange.y)
                    {
                        zoomPos = cameraPositions[_currentTarget].zoomRange.y;
                    }
                    _newZoomInput = true;
                }
            }

            orbitTarget.parent = cameraPositions[_currentTarget].camPos;
            orbitTarget.position = Vector3.Lerp(orbitTarget.position, cameraPositions[_currentTarget].camPos.position, Time.deltaTime * 20.0f);
            if(!_newOrbitInput | !cameraPositions[_currentTarget].canOrbit)
            {
                orbitTarget.rotation = Quaternion.Slerp(orbitTarget.rotation, cameraPositions[_currentTarget].camPos.rotation, Time.deltaTime * 20.0f);
            }
            else
            {
                orbitTarget.localRotation = Quaternion.Slerp(orbitTarget.localRotation, Quaternion.Euler(_orbitOriginRotation), Time.deltaTime * 20.0f);
            }
            if(!_newZoomInput | !cameraPositions[_currentTarget].canZoom)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0.0f, 0.0f, cameraPositions[_currentTarget].defaultZoomDistance), Time.deltaTime * 20.0f);
            }
            else
            {
                transform.localPosition = new Vector3(0.0f, 0.0f, zoomPos);
            }
        


            // //Orbiting
            // lastMousePos = currentMousePos;
            // currentMousePos = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            // float mouseVelX = (currentMousePos.x - lastMousePos.x) / Time.deltaTime;
            // float mouseVelY = (currentMousePos.y - lastMousePos.y) / Time.deltaTime;

            // Vector3 futureOrbitOriginRotation = orbitOriginRotation;
            // if (Input.GetMouseButton(0))
            // {
            //     futureOrbitOriginRotation.y += mouseVelX * orbitSpeed * Time.deltaTime;
            //     futureOrbitOriginRotation.x -= mouseVelY * orbitSpeed * Time.deltaTime;
            //     futureOrbitOriginRotation.x = Mathf.Clamp(futureOrbitOriginRotation.x, verticalOrbitRangeMin, verticalOrbitRangeMax);
            // }
            // orbitOriginRotation = futureOrbitOriginRotation;
            // orbitTarget.localRotation = Quaternion.Euler(orbitOriginRotation);

            // if (globalOrbit)
            // {
            //     globalOrbitTarget.rotation = Quaternion.identity;
            // }


            // //Zooming
            // Vector3 camDir = (orbitTarget.position - transform.position).normalized;
            // Vector3 futurePosition = transform.position;

            // //Zoom Out
            // if (Input.GetAxis("Mouse ScrollWheel") < 0)
            // {
            //     futurePosition -= camDir * zoomSpeed * Time.deltaTime;
            // }
            // //Zoom In
            // if (Input.GetAxis("Mouse ScrollWheel") > 0)
            // {
            //     futurePosition += camDir * zoomSpeed * Time.deltaTime;
            // }

            // //Make Sure The Camera Would Still Be Within Legal Values Before Applying The New Position
            // if (Vector3.Dot((orbitTarget.position - futurePosition).normalized, transform.forward) > 0.5f)
            // {
            //     if (Vector3.Distance(futurePosition, orbitTarget.position) < maxZoomDistance)
            //     {
            //         transform.position = futurePosition;
            //     }
            // }


            // transform.LookAt(orbitTarget); //Make Sure The Camera Is Always Facing The Origin
            // transform.localRotation = Quaternion.identity; //Prevent The Camera From Gimbal Locking
        }
    }
}
