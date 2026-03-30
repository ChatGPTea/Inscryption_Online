/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

namespace EnjoyGameClub.TextLifeFramework.Core
{
    public class StateMachine
    {
        public State State { get; private set; }

        public void ChangeState(State newState)
        {
            if (newState == null) return;

            //Exit old state.
            State?.Exit();
            //Set new state.
            State = newState;
            //Enter new state.
            State.Enter();
        }

        public void UpdateState()
        {
            State?.Update();
        }

        public void LateUpdateState()
        {
            State?.LateUpdate();
        }
    }
}