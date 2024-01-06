using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MyProject
{
	[RequireComponent(typeof(RectTransform))]
	public class TapButton : Selectable, IPointerDownHandler, ISubmitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[Serializable]
		public class ButtonTransition
		{
			public float targetLocalScaleX = 1f;
			public float targetLocalScaleY = 1f;

			public float smoothTime = 1f;
		}

		#region Inspector

		[Header("Selection")]
		[SerializeField] Color normalColor = Color.white; public Color NormalColor { get => normalColor; }
		[SerializeField] Color selectedColor = Color.white; public Color SelectedColor { get => selectedColor; }
		/// <summary> プレイヤーがボタンを押したまま、このピクセル数以上ドラッグすると、選択はキャンセルされる。 </summary>
		[SerializeField] float dragThreshold = 10.0f; public float DragThreshold { get => dragThreshold; set => dragThreshold = value; }

		[Header("Transitions")]
		[SerializeField] ButtonTransition normalTransition;
		[SerializeField] ButtonTransition downTransition;
		[SerializeField] ButtonTransition clickTransition;

		[Header("Button Events")]
		[SerializeField] UnityEvent onDown; public UnityEvent OnDown { get => onDown; }
		[SerializeField] UnityEvent onClick; public UnityEvent OnClick { get => onClick; }

		#endregion

		#region Fields

		protected Vector2 totalDelta;

		/// <summary> 現在ダウンしているポインターを追跡し、トランジションを切り替えられるようにする。 </summary>
		protected List<int> downPointers = new List<int>();

		protected RectTransform rectTransform;

		protected bool isSelected = false;

		#endregion

		#region Properties



		#endregion

		#region MonoBehaviour

		protected override void Awake()
		{
			TryGetComponent<RectTransform>(out rectTransform);
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);

			// ポインター追跡リストにポインターを追加
			downPointers.Add(eventData.pointerId);

			if (downTransition != null)
				StartCoroutine(UpdateLocalScaleXY(rectTransform, downTransition.targetLocalScaleX, downTransition.targetLocalScaleY, downTransition.smoothTime));

			onDown?.Invoke();
		}

		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);

			// ポインター追跡リストから現在ダウンしているポインターを削除できれば
			if (downPointers.Remove(eventData.pointerId) == true)
			{
				if (normalTransition != null)
					StartCoroutine(UpdateLocalScaleXY(rectTransform, normalTransition.targetLocalScaleX, normalTransition.targetLocalScaleY, normalTransition.smoothTime));

				DoClick();
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			//Debug.Log("ドラッグ開始！");
		}

		public void OnDrag(PointerEventData eventData)
		{
			// 現在ダウンしているポインターがポインター追跡リスト存在していれば
			if (downPointers.Contains(eventData.pointerId) == true)
			{
				totalDelta += eventData.delta;

				if (dragThreshold > 0.0f && totalDelta.magnitude > dragThreshold)
				{
					downPointers.Remove(eventData.pointerId);

					if (normalTransition != null)
						StartCoroutine(UpdateLocalScaleXY(rectTransform, normalTransition.targetLocalScaleX, normalTransition.targetLocalScaleY, normalTransition.smoothTime));
				}
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			//Debug.Log("ドラッグ終了！");
		}

		public void OnSubmit(BaseEventData eventData)
		{
			//Debug.Log("提出！");
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			// ボタンから指が離れている
			if (dragThreshold == 0.0f)
			{
				downPointers.Remove(eventData.pointerId);

				if (normalTransition != null)
					StartCoroutine(UpdateLocalScaleXY(rectTransform, normalTransition.targetLocalScaleX, normalTransition.targetLocalScaleY, normalTransition.smoothTime));
			}
		}

		#endregion

		#region Methods

		protected virtual void DoClick()
		{
			if (clickTransition != null)
				StartCoroutine(UpdateLocalScaleXY(rectTransform, clickTransition.targetLocalScaleX, clickTransition.targetLocalScaleY, clickTransition.smoothTime));

			onClick?.Invoke();
        }

		public virtual void DoSelect()
		{
			isSelected = true;
			targetGraphic.color = selectedColor;
		}

		public virtual void Deselect()
		{
			isSelected = false;
			targetGraphic.color = normalColor;
		}

		private IEnumerator UpdateLocalScaleXY(RectTransform rect, float targetX, float targetY, float smoothTime)
		{
			// スケールのX・Yの値が目標のスケールのX・Yの値と異なれば
			while (rect.localScale.x != targetX || rect.localScale.y != targetY)
			{
				float newScaleX = Mathf.Lerp(rect.localScale.x, targetX, smoothTime);
				float newScaleY = Mathf.Lerp(rect.localScale.y, targetY, smoothTime);

				rect.localScale = new Vector3(newScaleX, newScaleY, 1f);

				yield return null;
			}
		}

		#endregion
	}
}
