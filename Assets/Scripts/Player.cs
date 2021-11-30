using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    public float m_HorizontalSpeed = 0.0f;
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
    public float m_JumpWallSpeed = 2.0f;
    float m_time = 0.0f;
    public float m_GravityMultiplayer = 1.2f;
    public float m_MAxGravityMultiplayer = 4f;
    bool m_IncreaseGravity = false;


    Vector3 l_Movement = Vector3.zero;
    [Header("WallJump")]
    public bool m_IsWallJumping = false;

    [SerializeField] float m_friction = 1f;
    [SerializeField] float m_WallMaxJumpTime = 1f;
    IEnumerator m_resetWallJump;
    Vector3 m_dir;

    void Awake()
    {
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchControllerTransform.localRotation.eulerAngles.x;
        m_CharacterController = GetComponent<CharacterController>();

       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {//ROTACION CAMARA input
        float l_MouseAxisY = Input.GetAxis("Mouse Y");
        float l_MouseAxisX = Input.GetAxis("Mouse X");
        //m_pitch es x en mru x = x0+v*t
        m_Pitch += l_MouseAxisY * m_PitchRotationalSpeed * Time.deltaTime * (m_InvertedPitch ? -1.0f : 1.0f);
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        m_Yaw += l_MouseAxisX * m_YawRotationalSpeed * Time.deltaTime * (m_InvertedYaw ? -1.0f : 1.0f);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0);
        m_PitchControllerTransform.localRotation = Quaternion.Euler(m_Pitch, 0.0f, m_PitchControllerTransform.localRotation.eulerAngles.z);
        
        l_Movement = Vector3.zero;
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));
        if (Input.GetKey(m_UpKeyCode))
                l_Movement += l_Forward;
            else if (Input.GetKey(m_DownKeyCode))
                l_Movement = -l_Forward;
            if (Input.GetKey(m_RightKeyCode))
                l_Movement += l_Right;
            else if (Input.GetKey(m_LeftKeyCode))
                l_Movement -= l_Right;
            l_Movement.Normalize();

        l_Movement = l_Movement * Time.deltaTime * m_Speed;
        //IMPLEMENTACION DE GRAVEDAD
        if (!m_OnGround && (m_VerticalSpeed == 0 || (m_OldVerticalSpeed < 0 && m_VerticalSpeed > 0)))
        {
            m_IncreaseGravity = true;
        }

        //v = v0 +a*t
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime * (m_IncreaseGravity ? m_GravityMultiplayer : 1.0f);
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;
        if (m_HorizontalSpeed != 0)
        {
            m_HorizontalSpeed -= m_friction;
            if(m_HorizontalSpeed < 0)
            {
                m_HorizontalSpeed = 0.1f;
            }
            m_dir = l_Forward + m_WallRun.WallJump();
            m_dir.Normalize();
            l_Movement += m_dir * m_HorizontalSpeed;
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
                if (m_WallRun.m_IsWallRunning){
                    m_HorizontalSpeed = m_JumpWallSpeed;
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
}
