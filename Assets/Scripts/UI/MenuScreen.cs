using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MyProject
{
	[RequireComponent(typeof(CanvasGroup))]
	public class MenuScreen : MonoBehaviour
	{
		[Serializable]
		public class ScreenTransition
		{
			public float targetAlpha = 1f;

			public float duration = 1f;
		}

		#region Inspector

		[Header("Transitions")]
		[SerializeField] ScreenTransition onTransition;
		[SerializeField] ScreenTransition offTransition;

		[Header("Screen Events")]
		[SerializeField] UnityEvent onOn; public UnityEvent OnOn { get => onOn; }
		[SerializeField] UnityEvent onOff; public UnityEvent OnOff { get => onOff; }

		#endregion

		#region Fields

		[Header("Screen Behaviour")]
		public bool HideOnStart = true;

		protected CanvasGroup canvasGroup;

		#endregion

		#region Properties



		#endregion

		#region MonoBehaviour

		protected virtual void Awake()
		{
			TryGetComponent<CanvasGroup>(out canvasGroup);
		}

		protected virtual void Start()
		{
			if (HideOnStart)
				TurnOff();
			else
				TurnOn();
		}

		#endregion

		#region Methods

		public virtual void TurnOn()
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;

			if (onTransition != null)
				StartCoroutine(UpdateAlpha(canvasGroup, onTransition.targetAlpha, onTransition.duration));

			onOn?.Invoke();
		}

		public virtual void TurnOff()
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;

			if (offTransition != null)
				StartCoroutine(UpdateAlpha(canvasGroup, offTransition.targetAlpha, offTransition.duration));

			onOff?.Invoke();
		}

		private IEnumerator UpdateAlpha(CanvasGroup canvasGroup, float targetAlpha, float duration)
		{
			// α値が目標のα値と異なれば
			while (canvasGroup.alpha != targetAlpha)
			{
				float newAlpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, duration);

				canvasGroup.alpha = newAlpha;

				yield return null;
			}
		}

		#endregion
	}
}
