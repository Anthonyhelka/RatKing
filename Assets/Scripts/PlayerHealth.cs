﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

  private Animator _animator;
  private Rigidbody2D _rb;
  private PlayerController _playerControllerScript;

  public int health;
  public int heartCount;
  public Image[] hearts;
  public Sprite fullHeart;
  public Sprite emptyHeart;
  public GameObject gameOverText;

  private float  _invincibilityCooldown = 2.0f;
  private float _invincibilityTimer = -1.0f;

  private float _vomitDuration = 3.4f;
  private float _spikesDuration = 1.9f;

  [SerializeField] public bool _damaged;
  public bool Damaged {
    get { return _damaged; }
    set {
      if (value == _damaged) return;
      _damaged = value;
      _animator.SetBool("damaged", _damaged);
    }
  }

  [SerializeField] public bool _dying;
  public bool Dying {
    get { return _dying; }
    set {
      if (value == _dying) return;
      _dying = value;
      _animator.SetBool("dying", _dying);
    }
  }

  [SerializeField] public bool _vomit;
  public bool Vomit {
    get { return _vomit; }
    set {
      if (value == _vomit) return;
      _vomit = value;
      _animator.SetBool("vomit", _vomit);
    }
  }

  [SerializeField] public bool _spikes;
  public bool Spikes {
    get { return _spikes; }
    set {
      if (value == _spikes) return;
      _spikes = value;
      _animator.SetBool("spikes", _spikes);
    }
  }

  void Awake() {
    _rb = GetComponent<Rigidbody2D>();
    _animator = GetComponent<Animator>();
    _playerControllerScript = GetComponent<PlayerController>();
    gameOverText = GameObject.Find("Game_Over");
    gameOverText.SetActive(false);
  }

  void Update() {
    HealthUI();
  }

  void HealthUI() {
    if (health > heartCount) {
      health = heartCount;
    }
    for (int i = 0; i < hearts.Length; i++) {
      if (i < health) {
        hearts[i].sprite = fullHeart;
      } else {
        hearts[i].sprite = emptyHeart;
      }

      if (i < heartCount) {
        hearts[i].enabled = true;
      } else {
        hearts[i].enabled = false;
      }
    }
  }

  public void TakeDamage(string enemyTag) {
    if (Time.time < _invincibilityTimer) return;

    health--;

    StartCoroutine(DamagerPlayerRoutine(enemyTag));
  }

  IEnumerator DamagerPlayerRoutine(string enemyTag) {
    Damaged = true;
    _invincibilityTimer = Time.time + _invincibilityCooldown;
    _playerControllerScript._lockPlayerInput = true;

    _rb.velocity = new Vector2(0, 0) * 0;
    _rb.gravityScale = 1.0f;

    float duration = 0.0f;
    while (duration < 0.2f && !Dying) {
      duration += Time.deltaTime;
      if (_playerControllerScript._facingRight == false) {
        _rb.AddForce(Vector2.right * 0.2f, ForceMode2D.Impulse);
      } else {
        _rb.AddForce(Vector2.left * 0.2f, ForceMode2D.Impulse);
      }
      yield return 0;
    }

    Damaged = false;
    _playerControllerScript._lockPlayerInput = false;

    if (health <= 0) {
      StartCoroutine(PlayerDeathRoutine(enemyTag));
    }
  }

  IEnumerator PlayerDeathRoutine(string enemyTag) {
    Dying = true;
    _playerControllerScript._lockPlayerInput = true;
    
    float maxDuration = 0.0f;
    if (enemyTag == "Infected") {
      maxDuration = _vomitDuration;
      Vomit = true;
    }
    if (enemyTag == "Spikes") {
      maxDuration = _spikesDuration;
      Spikes = true;
    }

    float duration = 0.0f;
    while (duration < maxDuration) {
      duration += Time.deltaTime;
      yield return 0;
    }

    gameOverText.SetActive(true);
  }
}
