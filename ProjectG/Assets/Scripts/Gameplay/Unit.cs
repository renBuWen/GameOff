﻿using UnityEngine;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using RootMotion.Dynamics;

using Limb = UnitLimb.LimbType;
using Object = UnityEngine.Object;

public abstract class Unit
{
    //-- VARIABLES --------------------------------------------------------------
    // -- Data
    public UnitData Data;

    // -- Object
    public GameObject UnitParentObj;
    public GameObject UnitObj;

    // -- Animators
    protected Animator UnitAnimator;
    protected FullBodyBipedIK BodyIK;

    // -- Gameplay Vars
    // To-DO: Expose a scriptable object or something (conect to Unit Data)
    private float m_RotationSpeed = 0.2f;

    public bool HasFinishedTurn
    {
        get
        {
            if(p_CommandQueue == null) return false;
            return p_CommandQueue.Count >= Data.MaxQueueInput;
        }
    }
    protected int p_CommandsQueued
    {
        get
        {
            if(p_CommandQueue == null) return 0;
            return p_CommandQueue.Count;
        }
    }

    protected Queue<QueuedCommand> p_CommandQueue = new Queue<QueuedCommand>();
    private float m_QueueTimer;
    private float m_QueueTrigger;

    // -- Movement Vars
    private float m_CurrentRotAngle;
    private float m_TargetRotAngle;
    private float m_TargetRotVel;

    // -- IK Vars

    //-- CONSTRUCTOR -------------------------------------------------------------

    public Unit(string prefabName)
    {
        UnitParentObj = Object.Instantiate(Resources.Load<GameObject>(prefabName));
        UnitAnimator = UnitParentObj.GetComponentInChildren<Animator>();
        UnitObj = UnitAnimator.gameObject;
        BodyIK = UnitObj.GetComponent<FullBodyBipedIK>();
    }

    //-- UPDATE ----------------------------------------------------------------

    public void Update()
    {

    }

    //-- COMMAND QUEUE -----------------------------------------------------------

    public void UpdateQueue()
    {
        QueueLogic();
    }

    public void ResetQueue()
    {
        m_QueueTrigger = 0;
        m_QueueTimer = 0;
        p_CommandQueue.Clear();
    }

    protected virtual void QueueLogic()
    {
        m_QueueTimer += Time.deltaTime;
        if(m_QueueTimer > m_QueueTrigger)
        {
            QueuedCommand command = p_CommandQueue.Dequeue();
            command.Execute();
            command = null;
            m_QueueTrigger = p_CommandQueue.Peek().ExecutionTime;
            m_QueueTimer = 0;
        }
    }

  //-- COMMANDS ----------------------------------------------------------------

    #region Commands

    public void MoveCommand(float x, float y)
    {
        if(UnitAnimator == null) return;

        Vector3 dir = new Vector3(y, 0, -x);
        float speed = dir.magnitude;

        UnitAnimator.SetFloat(AnimationID.MoveSpeed, speed);

        if(dir.x != 0 || dir.z != 0)
        {
            m_TargetRotAngle = Vector3.Angle(-Vector3.forward, dir);

            Vector3 cross = Vector3.Cross(Vector3.forward, dir);
            if(cross.y > 0)
            {
                m_TargetRotAngle = 360 - m_TargetRotAngle;
            }

            m_TargetRotAngle = Utility.ClampToCircle(m_TargetRotAngle + 90);
        }

        m_CurrentRotAngle = Mathf.SmoothDampAngle(m_CurrentRotAngle, m_TargetRotAngle, ref m_TargetRotVel, m_RotationSpeed);
        UnitObj.transform.rotation = Quaternion.Euler(0, m_CurrentRotAngle, 0);


        //m_TargetRotAngle = Vector3.Angle(UnitObj.transform.forward, dir) - 90;

        //Vector3 cross = Vector3.Cross(UnitObj.transform.forward, dir);
        //if(cross.z > 0)
        //{
        //    m_TargetRotAngle = 360 - m_TargetRotAngle;
        //}

        //m_CurrentRotAngle = Mathf.SmoothDamp(m_CurrentRotAngle, m_TargetRotAngle, ref m_TargetRotVel, m_RotationSpeed);

        //Debugger.Log("Target Angle: " + Mathf.Round(m_TargetRotAngle), DebuggerTags.DBTag.Testing);
        ////Debugger.Log("Current Angle: " + m_CurrentRotAngle, DebuggerTags.DBTag.Testing);

        //UnitObj.transform.Rotate(0, m_TargetRotAngle, 0);
    }

    public void QueueAttackCommand(Limb limb)
    {
        if(HasFinishedTurn) return;
        Limb l = limb;
        QueuedCommand com = new QueuedCommand(()=> AttackCommand(l), 1); // To-Do Read attack data
        p_CommandQueue.Enqueue(com);
    }

    public void AttackCommand(Limb limb)
    {
        string animID = AnimationID.GetAttackLimb(limb);
        if(animID == null) return;

        UnitAnimator.SetTrigger(animID);
    }

    #endregion

    //-- IK FUNCTIONS ------------------------------------------------------------

    public void UpdateIK()
    {

    }


    //-- HELPER FUNCTIONS --------------------------------------------------------


    //----------------------------------------------------------------------------

    protected class QueuedCommand
    {
        private Action m_Command;
        public float ExecutionTime { get; private set; }

        public QueuedCommand(Action command, float executionTime)
        {
            m_Command = command;
            ExecutionTime = executionTime;
        }

        public void Execute()
        {
            m_Command();
        }
    }
}
