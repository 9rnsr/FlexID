[{Tc}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   OralcavityRe          O-cavity
  acc   Oesophagus-sRe        Oesophag-s
  acc   St-wall               St-wall
  acc   St-conRe              St-cont
  acc   RC-wall               RC-wall

  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   S-glands              S-glands
  acc   Thyroid1              Thyroid       # fast turnover
  acc   Thyroid2              Thyroid       # slow turnover
  acc   Liver1                Liver         # fast turnover
  acc   Liver2                Liver         # slow turnover
  acc   Kidneys1              Kidneys       # Urinary path
  acc   Kidneys2              Kidneys       # Other kidney tissues
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other


[{Tc}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.130 p.76 Table 3.4
# ICRP Publ.130 p.77 Para.143
# ICRP Publ.134 p.334 Fig.15.1
  OralcavityRe            Oesophagus-sRe          720
  Oesophagus-sRe          St-conRe               2160
  St-conRe                SI-conRe                 20.57

# ICRP Publ.134 p.334 Table 15.3
  Blood                   Thyroid1                  7.0
  Blood                   ST0                      71.88
  Blood                   ST1                       3.0
  Blood                   ST2                       0.18
  Blood                   UB-con                    1.7
  Blood                   S-glands                  2.6
  Blood                   St-wall                   4.3
  Blood                   Kidneys1                  0.7
  Blood                   Kidneys2                  0.04
  Blood                   Liver1                    4.5
  Blood                   RC-wall                   3.4
  Blood                   T-bone-S                  0.35
  Blood                   C-bone-S                  0.35
  Thyroid1                Blood                   100
  Thyroid1                Thyroid2                  1.0
  Thyroid2                Blood                     1.0
  ST0                     Blood                    50
  ST1                     Blood                     0.462
  ST2                     Blood                     0.0347
  S-glands                OralcavityRe             50
  St-wall                 St-conRe                 50
  Kidneys1                UB-con                    8.32
  Kidneys2                Blood                     0.0347
  Liver1                  Blood                     8.234
  Liver1                  Liver2                    0.0832
$if parent
  Liver2                  Blood                     0.0347      # only for progeny of Mo
$endif
  RC-wall                 RC-con                    1.39
  T-bone-S                Blood                     0.457
  T-bone-S                T-bone-V                  0.00462
  C-bone-S                Blood                     0.457
  C-bone-S                C-bone-V                  0.00462
  T-bone-V                Blood                     0.000493
  C-bone-V                Blood                     0.0000821

$if progeny

  {Tc}/OralcavityRe       OralcavityRe              ---
  {Tc}/Oesophagus-sRe     Oesophagus-sRe            ---
  {Tc}/St-wall            St-wall                   ---
  {Tc}/St-conRe           St-conRe                  ---
  {Tc}/RC-wall            RC-wall                   ---

# ICRP Publ.134 p.313 Para.664
# > (664) External measurements on normal human subjects indicated that 99mTc
# > produced in the liver by decay of 99Mo following intravenous administration of
# > 99Mo as sodium or ammonium molybdate was retained in the liver for an extended
# > period (Sorensen and Archambault, 1963). In contrast, 99mTc depositing in the liver
# > after administration as a parent radionuclide was largely removed with a half-time of
# > a few hours (Sorensen and Archambault, 1963). On the basis of these findings,
# > technetium produced in the liver by decay of a molybdenum parent is assigned to
# > the long-term retention compartment of liver in the characteristic model for technetium
# > described elsewhere in this publication. The removal half-time from that compartment
# > to blood is 22 d. For modelling convenience, the compartment of the
# > molybdenum model called Blood 1 is identified with the blood compartment of
# > the technetium model. Technetium produced in the compartment Blood 2 of the
# > molybdenum model is assumed to transfer to blood in the technetium model at a
# > rate of 1000 d⁻¹ (t½=1 min). Technetium produced in a compartment of kidneys or
# > ‘other tissue’ in the molybdenum model is assumed to transfer to blood in the technetium
# > model at a rate of 0.462 d⁻¹; the rate of transfer to blood from the intermediate-
# > term compartment of ‘other tissue’ in the technetium model. After reaching
# > blood, technetium is assumed to follow its characteristic model.
# > (664) 健常人を対象とした外部測定では、モリブデン酸ナトリウムまたはモリブデン酸アンモニウムとして
# > 静脈内投与された99Moの崩壊により肝臓で生成された99mTcは、長期間肝臓に保持されることが
# > 示唆されました（Sorensen and Archambault, 1963）。対照的に、親放射性核種として投与された後に
# > 肝臓に沈着した99mTcは、数時間の半減期で大部分が除去されました（Sorensen and Archambault, 1963）。
# > これらの知見に基づき、モリブデン親核種の崩壊により肝臓で生成されたテクネチウムは、
# > この刊行物の別の箇所で説明されているテクネチウムの特性モデルにおける肝臓の
# > 長期保持コンパートメントに割り当てられます。このコンパートメントから血液への除去半減期は
# > 22日です。モデル化の便宜上、モリブデンモデルの Blood 1 と呼ばれるコンパートメントは、
# > テクネチウムモデルの血液コンパートメントと同一視されます。モリブデンモデルの
# > Blood 2 コンパートメントで生成されたテクネチウムは、テクネチウムモデルの血液へ
# > 1000 d⁻¹（t½=1分）の速度で移行すると仮定されます。モリブデンモデルの腎臓または
# > 「その他の組織」コンパートメントで生成されたテクネチウムは、テクネチウムモデルの血液へ
# > 0.462 d⁻¹の速度で移行すると仮定されます。これは、テクネチウムモデルの「その他の組織」の
# > 中間コンパートメントから血液への移行速度です。血液に到達したテクネチウムは、
# > その特性モデルに従うと仮定されます。

  Liver2                  Blood                 $(0.693 / 22)   # 半減期22日

  {Mo}/Blood1             Blood                     ---
  {Mo}/Blood2             Blood                  1000           # 半減期1分
  {Tc}/Blood              Blood                     ---

  {Tc}/C-bone-S           C-bone-S                  ---
  {Tc}/T-bone-S           T-bone-S                  ---
  {Tc}/C-bone-V           C-bone-V                  ---
  {Tc}/T-bone-V           T-bone-V                  ---

  {Mo}/Liver              Liver2                    ---
  {Mo}/UrinaryPath        Blood                     0.462
  {Mo}/OtherKidneyTis     Blood                     0.462
  {Tc}/S-glands           S-glands                  ---
  {Tc}/Thyroid1           Thyroid1                  ---
  {Tc}/Thyroid2           Thyroid2                  ---
  {Tc}/Liver1             Liver1                    ---
  {Tc}/Liver2             Liver2                    ---
  {Tc}/Kidneys1           Kidneys1                  ---
  {Tc}/Kidneys2           Kidneys2                  ---

  {Mo}/OtherTissue        Blood                     0.462
  {Tc}/ST0                ST0                       ---
  {Tc}/ST1                ST1                       ---
  {Tc}/ST2                ST2                       ---

$endif
