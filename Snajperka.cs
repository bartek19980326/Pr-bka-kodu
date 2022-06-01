
using System.Collections;
using UnityEngine;

public class Snajperka : Bronie
{
  [SerializeField]
  private GameObject celownikSnajpera;
  [SerializeField]
  private Camera mainCamera;
  [SerializeField]
  private GameObject celownik;
  [SerializeField]
  private float normalFOV;
  [SerializeField]
  private GameObject kameraBroni;
  [SerializeField]
  private float FOV;
  [SerializeField]
  private bool isScoped;
  [SerializeField]
  private new AudioClip Dzwiek;
  public new AudioClip Przeladowanie;

  public override void Start()
  {
    base.Start();
    this.CurrentAmmoText = Graczmenager.instance.CurrentAmmo;
    this.fpscamera = Graczmenager.instance.GunCamera;
    this.MaxAmmoText = Graczmenager.instance.maxammo;
    this.impactEffect = Graczmenager.instance.Uderzenie;
    this.anim = Graczmenager.instance.Bron;
    this.pkt = Graczmenager.instance.wynik;
    this.celownikSnajpera = Graczmenager.instance.CelownikSnajpera;
    this.mainCamera = Graczmenager.instance.MainCamera;
    this.celownik = Graczmenager.instance.Celowanik;
    this.kameraBroni = Graczmenager.instance.GunCameraObject;
    this.Dzwiek = Graczmenager.instance.dzwiek;
    this.Przeladowanie = Graczmenager.instance.Przeladowanie;
  }

  public override void Update()
  {
    if (this.isReloading)
      return;
    if (this.currentAmmo <= 0)
    {
      this.StartCoroutine(this.Reload());
      AudioManager.Instance.Play(this.Przeladowanie);
    }
    else if ((double) this.currentAmmo < 10.0 && Input.GetKey("r"))
    {
      this.StartCoroutine(this.Reload());
      AudioManager.Instance.Play(this.Przeladowanie);
    }
    if (Input.GetButtonDown("Fire1") && (double) Time.time >= (double) this.nextTimeToFire)
    {
      this.nextTimeToFire = Time.time + 1f / this.szybkostrzelnosc;
      this.Shoot();
      AudioManager.Instance.Play(this.Dzwiek);
    }
    this.Scope();
    this.ZliczAmunicje();
  }

  public void Scope()
  {
    if (!Input.GetButtonDown("Fire2"))
      return;
    this.isScoped = this.anim.GetBool("Scoped");
    this.isScoped = !this.isScoped;
    this.anim.SetBool("Scoped", this.isScoped);
    if (this.isScoped)
      this.StartCoroutine(this.onScoped());
    else
      this.onUnscoped();
  }

  public IEnumerator onScoped()
  {
    yield return (object) new WaitForSeconds(0.15f);
    this.celownikSnajpera.SetActive(true);
    this.kameraBroni.SetActive(false);
    this.celownik.SetActive(false);
    this.normalFOV = this.mainCamera.fieldOfView;
    this.mainCamera.fieldOfView = this.FOV;
  }

  public void onUnscoped()
  {
    this.celownikSnajpera.SetActive(false);
    this.celownik.SetActive(true);
    this.kameraBroni.SetActive(true);
    this.mainCamera.fieldOfView = 60f;
  }
}
