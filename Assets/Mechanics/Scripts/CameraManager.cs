using UnityEngine;
using TMPro;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CameraUpdateType updateType;

    [Header("Specs")]
    [Space(5)]

    [SerializeField] private bool follow;
    [SerializeField] private bool rotate;
    [SerializeField] private bool rotateAllAxis;
    [SerializeField] private bool rotateWithTarget;

    [SerializeField] private bool raceMode = false;

    [Header("Transforms")]
    [Space(5)]

    [SerializeField] private Transform positionTarget;
    [SerializeField] private Transform rotationTarget;

    [Header("Values")]
    [Space(5)]

    [SerializeField] private float xDistance;
    [SerializeField] private float yDistance;
    [SerializeField] private float zDistance;

    [Space(5)]

    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [SerializeField] private float zOffset;

    [Space(5)]

    [SerializeField] private float lookAngle;

    [Space(5)]

    [SerializeField] private float rotationSmooth;
    [SerializeField] private float positionSmooth;


    [SerializeField] private ParticleSystem confetti;

    [HideInInspector] public Camera cam;

    [SerializeField] private Transform follower;
    [SerializeField] private Transform car;

    private Vector3 refVelocity;
    private int y = 0;

    #region Singleton
    public static CameraManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        cam = GetComponent<Camera>();
    }
    #endregion

    public void PlayConfetti()
    {
        confetti.gameObject.SetActive(true);
        confetti?.Play();
    }
    public void StopConfetti()
    {
        confetti?.Stop();
        confetti.gameObject.SetActive(false);
    }

    private void FixedUpdate() { if (updateType == CameraUpdateType.Fixed) CameraFocus(); }
    private void LateUpdate() { if (updateType == CameraUpdateType.Late) CameraFocus(); }
    private void Update() { if (updateType == CameraUpdateType.Normal) CameraFocus(); }
    private void CameraFocus()
    {
        if (!positionTarget) return;

        if (raceMode)
        {
            float wantedRotationAngle = positionTarget.eulerAngles.y;
            float wantedHeight = positionTarget.position.y + (yDistance);
            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, 3 * Time.deltaTime);
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, 2 * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
            transform.position = positionTarget.position;
            transform.position -= currentRotation * Vector3.forward * (zDistance);
            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
            transform.LookAt(positionTarget);

            return;
        }


        Vector3 worldposition = (Vector3.forward * xDistance) + (Vector3.up * yDistance) + (Vector3.right * zDistance);
        Vector3 rotatedvector = Quaternion.AngleAxis(lookAngle + (rotateWithTarget ? rotationTarget.localEulerAngles.y : 0), Vector3.up) * worldposition;
        Vector3 flattargetposition = positionTarget.position;
        Vector3 finalposition = flattargetposition + rotatedvector + positionTarget.up * yOffset + positionTarget.right * xOffset + positionTarget.forward * zOffset;


        if (follow) transform.position = Vector3.SmoothDamp(transform.position, finalposition, ref refVelocity, positionSmooth);

        if (!rotationTarget) return;

        if (rotate)
        {
            Vector3 lookPos = (rotationTarget.position + new Vector3(xOffset, yOffset, zOffset) - transform.position);

            if (!rotateAllAxis) lookPos.y = 0;

            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = rotation;
        }
    }
    public void SetTarget(Transform positionT, Transform rotationT = null)
    {
        positionTarget = positionT;
        rotationTarget = rotationT == null ? positionT : rotationT;
    }
    public void SetOffset(float ofx, float ofy, float ofz)
    {
        xOffset = ofx;
        yOffset = ofy;
        zOffset = ofz;
    }
    public void SetValues(float x, float y, float z)
    {
        xDistance = x;
        yDistance = y;
        zDistance = z;
    }
    public void SetValues(float x, float y, float z, float angle)
    {
        xDistance = x;
        yDistance = y;
        zDistance = z;
        lookAngle = angle;
    }
    public void SetSpecs(bool _follow = true, bool _rotate = true)
    {
        follow = _follow;
        rotate = _rotate;
    }

    private int set = 0;

    public void ChangeCameraSettings()
    {
        set++;
        if (set > 4) set = 0;
        switch (set)
        {
            case 0: CameraSettingsMain(); break;
            case 1: CameraSettingsOne(); break;
            case 2: CameraSettingsTwo(); break;
            case 3: CameraSettingsThree(); break;
            case 4: CameraSettingsFour(); break;
        }
    }
    private void CameraSettingsMain()
    {
        SetValues(0, 0.5f, 3f);

        SetTarget(car);

        positionSmooth = 0.1f;
        rotationSmooth = 2f;

        raceMode = true;
    }
    private void CameraSettingsOne()
    {
        SetValues(10, 10, 0);

        SetTarget(follower);
        
        positionSmooth = 0.1f;
        rotationSmooth = 2f;

        raceMode = false;
    }
    private void CameraSettingsTwo()
    {
        SetValues(0, 2, 5);

        SetTarget(car);

        positionSmooth = 0.1f;
        rotationSmooth = 2f;

        raceMode = true;
    }
    private void CameraSettingsThree()
    {
        SetValues(0, 30, 5);

        SetTarget(follower);

        positionSmooth = 0.1f;
        rotationSmooth = 2f;

        raceMode = true;
    }
    private void CameraSettingsFour()
    {
        SetValues(-15, 10, 0);

        SetTarget(follower);

        positionSmooth = 0.1f;
        rotationSmooth = 2f;

        raceMode = false;
    }
    [System.Serializable]
    public enum CameraUpdateType
    {
        Fixed,
        Late,
        Normal,
    }
}