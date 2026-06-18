using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Game
{
    public class ParallaxEffect : MonoBehaviour
    {
        float _startPos, _length;
        [SerializeField] GameObject _cam;
        [SerializeField] float _parallaxEffect;//0 = move with cam, 1 = wont move, 0.5 = half

        void Start()
        {
            _startPos = transform.position.x;
            _length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        void LateUpdate()
        {
            float distance = _cam.transform.position.x * _parallaxEffect;
            float movement = _cam.transform.position.x * (1 - _parallaxEffect);

            transform.position = new Vector3(_startPos + distance,transform.position.y,transform.position.z);

            //infinite scroll
            if(movement > _startPos + _length)
            {
                _startPos += _length;
            }
            else if(movement < _startPos - _length)
            {
                _startPos -= _length;
            }
        }
    }
}

