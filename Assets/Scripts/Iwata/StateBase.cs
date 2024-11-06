using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
	public abstract class StateBase
	{
		// �uvirtual �L�[���[�h�v�͔h���N���X�ŃI�[�o�[���C�h�\�Ƃ����Ӗ�


		/// <summary>
		/// �X�e�[�g�J�n���ɌĂ΂�鏈��
		/// </summary>
		/// <param name="owner"> �N���Ă񂾂� </param>
		/// <param name="prevState"> ��O�̃X�e�[�g </param>
		public virtual void OnEnter(StateController owner, StateBase prevState) { }


		/// <summary>
		/// ���t���[����UpDate�ŌĂ΂�鏈��  
		/// </summary>
		/// <param name="owner"> �N���Ă񂾂� </param>
		public virtual void OnUpdate(StateController owner) { }


		/// <summary>
		/// �{�^���N���b�N���ɌĂ΂�鏈��
		/// </summary>
		/// <param name="owner"> �N���Ă񂾂� </param>
		/// <param name="nextState"> ���ɑJ�ڂ���X�e�[�g </param>
		public virtual void OnClick(StateController owner) { }


		/// <summary>
		/// �X�e�[�g�I�����ɌĂ΂�鏈��
		/// </summary>
		/// <param name="owner"> �N���Ă񂾂� </param>
		/// <param name="nextState"> ���ɑJ�ڂ���X�e�[�g </param>
		public virtual void OnExit(StateController owner, StateBase nextState) { }

	}
}