﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace Clinic
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class Interactable : MonoBehaviour
    {
        public bool PlayerInside;
        protected FSM<Interactable> m_InteractableFSM;
        protected Collider2D m_Collider2D;
        protected Vector2 m_Collider2DSize;
        protected Vector2 m_Collider2DPosition;
        private bool m_FirstTime;

        protected virtual void Awake()
        {
            m_InteractableFSM = new FSM<Interactable>(this);
            m_InteractableFSM.TransitionTo<InteractableIdleState>();
            m_Collider2D = GetComponent<Collider2D>();
            m_Collider2DSize = m_Collider2D.bounds.size;
            m_Collider2DPosition = m_Collider2D.bounds.center;
            PlayerInside = false;
            m_FirstTime = true;
        }

        public void OnTransitionToNonInteractable()
        {
            m_InteractableFSM.TransitionTo<InteractingNonCancelableState>();
        }

        public void OnTransitionToIdleState()
        {
            m_InteractableFSM.TransitionTo<InteractableIdleState>();
        }

        protected abstract void OnInteract();

        protected abstract void OnCancelInteract();

        protected abstract void OnEnterZone();

        protected abstract void OnExitZone();

        protected void Update()
        {
            m_InteractableFSM.Update();
            PlayerInside = CheckPlayerInside();
        }

        protected bool CheckPlayerInside()
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(m_Collider2DPosition, m_Collider2DSize, 0f, Vector2.zero);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                    return true;
            }
            return false;
        }

        protected abstract class InteractableState : FSM<Interactable>.State
        {
            protected bool InteractButtonDown { get { return global::Utility.InputComfirm(ControlState.Action); } }
            protected bool CancelButtonDown { get { return global::Utility.InputCancel(ControlState.Storage) || global::Utility.InputCancel(ControlState.CraftingTable); } }
            public override void OnEnter()
            {
                base.OnEnter();
                //print(GetType().Name);
            }
        }

        protected class InteractableIdleState : InteractableState
        {
            public override void OnEnter()
            {
                base.OnEnter();
                if (Context.m_FirstTime)
                {
                    Context.m_FirstTime = false;
                    return;
                }
                Context.OnCancelInteract();

            }
            public override void Update()
            {
                base.Update();
                if (Context.PlayerInside)
                {
                    TransitionTo<InteractableActivateState>();
                    return;
                }
            }
        }

        protected class InteractableActivateState : InteractableState
        {
            public override void OnEnter()
            {
                base.OnEnter();
                Context.OnEnterZone();
            }

            public override void Update()
            {
                base.Update();
                if (InteractButtonDown)
                {
                    Context.OnInteract();
                    TransitionTo<InteractingState>();
                    return;
                }
                if (!Context.PlayerInside)
                {
                    TransitionTo<InteractableIdleState>();
                    return;
                }
            }

            public override void OnExit()
            {
                base.OnExit();
                Context.OnExitZone();
            }
        }

        protected class InteractingState : InteractableState
        {
            public override void Update()
            {
                base.Update();
                if (CancelButtonDown)
                {
                    TransitionTo<InteractableIdleState>();
                    return;
                }
                if (!Context.PlayerInside)
                {
                    TransitionTo<InteractableIdleState>();
                    return;
                }
            }

            public override void OnExit()
            {
                base.OnExit();
            }
        }

        protected class InteractingNonCancelableState : InteractableState
        {
            public override void OnExit()
            {
                base.OnExit();
            }
        }
    }

}
