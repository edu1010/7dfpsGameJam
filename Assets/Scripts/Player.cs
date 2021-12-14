using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Camera m_camera;
    float m_Yaw;
    float m_Pitch;
    [Header("Camera")]
    public float m_YawRotationalSpeed = 360.0f;
    public float m_PitchRotationalSpeed = 180.0f;
    public float m_MinPitch = -80.0f;
    public float m_MaxPitch = 50.0f;
    public Transform m_PitchControllerTransform;
    public bool m_InvertedYaw = false;
    public bool m_InvertedPitch = true;
    [Header("Controls")]
    public CharacterController m_CharacterController;
    public float m_Speed = 10.0f;
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_ReloadCode = KeyCode.R;
    [Header("Gravity")]

    //gravedad
    public float m_VerticalSpeed = 0.0f;
    float m_HorizontalSpeed = 0.0f;
    float m_OldVerticalSpeed = 0.0f;//verticalSpeed de hace un frame
    public bool m_OnGround = false;
    private bool m_isJumping;
    public WallRun m_WallRun;


    [Header("Run and jump")]
    //x(sin(yaw),0,cos yaw)
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public float m_FastSpeedMultiplier = 1.2f;
    
    public float m_JumpSpeed = 10.0f;
    
    float m_time = 0.0f;
    public float m_GravityMultiplayer = 1.2f;
    public float m_MAxGravityMultiplayer = 4f;
    bool m_IncreaseGravity = false;


    Vector3 l_Movement = Vector3.zero;
    [Header("WallJump")]
    public bool m_IsWallRun= false;
    public float m_JumpWallSpeed = 2.0f;
    bool m_CanControl = true;

    float m_friction = 1f;
    
    IEnumerator m_resetWallJump;
    Vector3 m_dir;
    public float m_SecnodsWihoutControl = 0.5f;

    [Header("Hook")]
    public float m_hookVelocityMultiPlayer = 2f;
    public float m_hookVelocityMax = 40f;
    public float m_hookVelocityMin = 10f;
    Vector3 m_HookTargetPos;
    States m_state = States.Normal;
    public float m_ReachedHookPos = 1f;
    [Header("Hook Animation")]
    public Transform m_HookTransform;
    private float m_HookSize = 1f;
    public float m_throwSpeed = 70f;

    enum States
    {
        Normal = 0,
        Fly,
        HoockShotThorw
    }

    void Awake()
    {
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchControllerTransform.localRotation.eulerAngles.x;
        m_CharacterController = GetComponent<CharacterController>();
        m_HookTransform.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
        m_state = States.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_state)
        {
            case (States.Normal):
                CameraMovement();
                HandleHoosShootStart();
                Movement();
                break;
            case (States.Fly):
                CameraMovement();
                HandleHookShotMovement();
                HandleHoosShootStart();
                break;
            case (States.HoockShotThorw):
                HandleHookShotThrow();
                CameraMovement();
                Movement();
                break;
        }
        

    }
    private void CameraMovement()
    {
        //ROTACION CAMARA input
        float l_MouseAxisY = Input.GetAxis("Mouse Y");
        float l_MouseAxisX = Input.GetAxis("Mouse X");
        //m_pitch es x en mru x = x0+v*t
        m_Pitch += l_MouseAxisY * m_PitchRotationalSpeed * Time.deltaTime * (m_InvertedPitch ? -1.0f : 1.0f);
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        m_Yaw += l_MouseAxisX * m_YawRotationalSpeed * Time.deltaTime * (m_InvertedYaw ? -1.0f : 1.0f);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0);
        m_PitchControllerTransform.localRotation = Quaternion.Euler(m_Pitch, 0.0f, m_PitchControllerTransform.localRotation.eulerAngles.z);

    }
    private void Movement()
    {
        
        l_Movement = Vector3.zero;
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));
        if (m_CanControl)
        {
            if (Input.GetKey(m_UpKeyCode))
                l_Movement += l_Forward;
            else if (Input.GetKey(m_DownKeyCode))
                l_Movement = -l_Forward;
            if (Input.GetKey(m_RightKeyCode))
                l_Movement += l_Right;
            else if (Input.GetKey(m_LeftKeyCode))
                l_Movement -= l_Right;
            l_Movement.Normalize();
        }

        l_Movement = l_Movement * Time.deltaTime * m_Speed;
        //IMPLEMENTACION DE GRAVEDAD
        if (!m_OnGround && (m_VerticalSpeed == 0 || (m_OldVerticalSpeed < 0 && m_VerticalSpeed > 0)))
        {
            m_IncreaseGravity = true;
        }

        //v = v0 +a*t
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime * m_GravityMultiplayer;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        if (m_HorizontalSpeed != 0)
        {
            /* m_HorizontalSpeed -= m_friction;
             if(m_HorizontalSpeed < 0)
             {
                 m_HorizontalSpeed = 0.1f;
             }*/
            l_Movement += m_dir * Time.deltaTime * m_HorizontalSpeed;
        }

        //comprobamos si estamos corriendo
        float l_SpeedMultiplier = 1.0f;
        if (Input.GetKey(m_RunKeyCode))
        {
            l_SpeedMultiplier = m_FastSpeedMultiplier;
        }

        l_Movement *= Time.deltaTime * m_Speed * l_SpeedMultiplier;
        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);//Macara binaria para saber como hemos chocado, por arriba abajo
        if ((l_CollisionFlags & CollisionFlags.Below) != 0)//Colisiona con el suelo
        {
            m_OnGround = true;
            if (!m_WallRun.m_IsWallRunning)
                m_isJumping = false;
            m_VerticalSpeed = 0.0f;
            m_HorizontalSpeed = 0.0f;
            m_time = Time.time;
            m_CanControl = true;
        }
        else
        {
            if (Time.time - m_time > 0.3)
            {
                m_OnGround = false;
            }
        }
        //Cuando toca el techo caiga
        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
            m_VerticalSpeed = 0.0f;

        if (Input.GetKeyDown(m_JumpKeyCode))
        {
            if (m_OnGround || m_WallRun.m_IsWallRunning)
            {
                m_VerticalSpeed = m_JumpSpeed;
                if (m_WallRun.m_IsWallRunning)
                {
                    m_HorizontalSpeed = m_JumpWallSpeed;
                    m_CanControl = false;
                    m_dir = l_Forward + m_WallRun.WallJump();
                    m_dir.y = 0;
                    m_dir.Normalize();
                    StartCoroutine(ReturnControlToPlayer());
                }
            }
            m_isJumping = true;
        }
        else
        {
            m_isJumping = false;
        }

        m_OldVerticalSpeed = m_VerticalSpeed;

    }


    private void HandleHoosShootStart()
    {
        if (Input.GetMouseButtonDown(1))
        {
           if( Physics.Raycast(m_camera.transform.position, m_camera.transform.forward,out RaycastHit l_raycastHit))
            {
                m_HookTargetPos = l_raycastHit.point;
                m_state = States.HoockShotThorw;
                m_HookTransform.gameObject.SetActive(true);
                m_HookTransform.localScale = Vector3.zero;
                m_HookSize = 0f;
            }
        }
    }
    private void HandleHookShotThrow()
    {
        m_HookTransform.LookAt(m_HookTargetPos);
        m_HookSize += m_throwSpeed * Time.deltaTime;
        m_HookTransform.localScale = new Vector3(1f, 1f, m_HookSize);
        if(m_HookSize >= Vector3.Distance(transform.position, m_HookTargetPos)){
            m_state = States.Fly;
            m_HookTransform.gameObject.SetActive(false);
        }

    }
    private void HandleHookShotMovement()
    {
        m_HookTransform.LookAt(m_HookTargetPos);
        Vector3 l_HookDir = (m_HookTargetPos - transform.position).normalized;
        float l_hookSpeed = Mathf.Clamp(Vector3.Distance(transform.position, m_HookTargetPos), m_hookVelocityMin,  m_hookVelocityMax);

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_HookDir * l_hookSpeed * m_hookVelocityMultiPlayer * Time.deltaTime);

        if(Vector3.Distance(transform.position,m_HookTargetPos)< m_ReachedHookPos ||
          ((l_CollisionFlags & CollisionFlags.Sides) != 0))
        {
            m_state = States.Normal;
        }
        
    }





    IEnumerator ReturnControlToPlayer()
    {
        yield return new WaitForSeconds(m_SecnodsWihoutControl);
        m_CanControl = true;
    }
}
