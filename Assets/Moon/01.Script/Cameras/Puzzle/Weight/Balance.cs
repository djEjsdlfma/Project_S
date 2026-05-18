using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Moon._01.Script.Cameras.Puzzle.Weight
{
    public class Balance : MonoBehaviour
    {
        [SerializeField] private int balancedWeight;
        [SerializeField] private Collider2D balanceCollider;
        [SerializeField] private float reFindTime = 0.2f;
        
        public UnityEvent balancedEvent;

        private float _timer;

        private readonly HashSet<Collider2D> _processedColliders = new HashSet<Collider2D>();
        private readonly Queue<Collider2D> _collidersToCheck = new Queue<Collider2D>();
        private readonly List<Collider2D> _overlapResults = new List<Collider2D>(16);

        private ContactFilter2D _contactFilter;

        private void Start()
        {
            _contactFilter = ContactFilter2D.noFilter;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= reFindTime)
            {
                _timer = 0f;
                int currentWeight = CalculateStackedWeight();
                if (currentWeight == balancedWeight)
                {
                    balancedEvent?.Invoke();
                }
            }
        }

        private int CalculateStackedWeight()
        {
            int totalMass = 0;
        
            _processedColliders.Clear();
            _collidersToCheck.Clear();

            _collidersToCheck.Enqueue(balanceCollider);
            _processedColliders.Add(balanceCollider);

            while (_collidersToCheck.Count > 0)
            {
                Collider2D currentCol = _collidersToCheck.Dequeue();
                Bounds bounds = currentCol.bounds;

                Vector2 center = new Vector2(bounds.center.x, bounds.center.y + bounds.extents.y + 0.05f);
                
                Vector2 size = new Vector2(bounds.size.x, 0.1f);

                float angle = currentCol.transform.eulerAngles.z;

                int hitCount = Physics2D.OverlapBox(center, size, angle, _contactFilter, _overlapResults);

                for (int i = 0; i < hitCount; i++)
                {
                    Collider2D hit = _overlapResults[i];
                    
                    if (_processedColliders.Add(hit))
                    {
                        if (hit.TryGetComponent(out WeightObj weightObj))
                        {
                            totalMass += weightObj.Weight;
                        }
                        _collidersToCheck.Enqueue(hit); 
                    }
                }
            }
            return totalMass;
        }
    }
}