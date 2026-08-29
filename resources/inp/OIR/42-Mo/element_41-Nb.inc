[{Nb}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver0                Liver
  acc   Liver1                Liver
  acc   Kidneys               Kidneys
  acc   ST0                   Other
  acc   ST1                   Other


[{Nb}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.294 Table 13.3
  Blood1                  Blood2                    3.2
  Blood1                  Liver0                    0.24
  Blood1                  Kidneys                   0.04
  Blood1                  ST0                       3.2
  Blood1                  ST1                       0.12
  Blood1                  UB-con                    0.88
  Blood1                  SI-conRe                  0.08
  Blood1                  T-bone-S                  0.12
  Blood1                  C-bone-S                  0.12
  Blood2                  Blood1                    1.39
  Liver0                  SI-conRe                  0.0578
  Liver0                  Blood1                    0.0578
  Liver0                  Liver1                    0.231
  Liver1                  Blood1                    0.005
  Kidneys                 Blood1                    0.005
  ST0                     Blood1                    1.39
  ST1                     Blood1                    0.01
  T-bone-S                Blood1                    0.000493
  T-bone-S                T-bone-V                  0.000247
  T-bone-V                Blood1                    0.000493
  C-bone-S                Blood1                    0.0000821
  C-bone-S                C-bone-V                  0.0000411
  C-bone-V                Blood1                    0.0000821

$if progeny

# ICRP Publ.134 p.314 Para.665
# > (665) No information was found on the behaviour of niobium produced in vivo
# > following intake of a molybdenum parent. For modelling convenience, the compartment
# > of the molybdenum model called Blood 1 is identified with the central blood
# > compartment of the characteristic model for niobium (also named Blood 1; see
# > Fig. 13.1). It is assumed that niobium produced in Blood 2 of the molybdenum
# > model transfers to Blood 1 of the niobium model at a rate of 1000 d⁻¹. Niobium
# > produced in a tissue compartment of the molybdenum model is assumed to transfer
# > to Blood 1 in the niobium model at a rate of 1.39 d⁻¹; the highest rate of transfer to
# > blood from an ‘other tissue’ compartment of the niobium model. After reaching
# > Blood 1 in the niobium model, niobium is assumed to follow its characteristic systemic
# > model.
# > (665) モリブデン親核種の摂取後に体内で生成されたニオブの挙動に関する情報は
# > 得られていません。モデリングの便宜上、モリブデンモデルの Blood 1 と呼ばれる
# > コンパートメントは、ニオブの特性モデルの中央血液コンパートメント（Blood 1とも呼ばれる、
# > Fig. 13.1 参照）と同一視されます。モリブデンモデルの Blood 2 で生成されたニオブは、
# > 1000 d⁻¹の速度でニオブモデルの Blood 1 に移動すると仮定されます。モリブデンモデルの
# > 組織コンパートメントで生成されたニオブは、ニオブモデルの Blood 1 に1.39 d⁻¹の速度で
# > 移動すると仮定されます。これは、ニオブモデルの「他の組織」コンパートメントから
# > 血液への最高移行速度です。ニオブモデルの Blood 1 に到達した後、ニオブはその
# > 特性全身モデルに従うと仮定されます。

  {Mo}/Blood1             Blood1                    ---
  {Mo}/Blood2             Blood1                 1000
  {Nb}/Blood1             Blood1                    ---
  {Nb}/Blood2             Blood2                    ---

  {Nb}/C-bone-S           C-bone-S                  ---
  {Nb}/T-bone-S           T-bone-S                  ---
  {Nb}/C-bone-V           C-bone-V                  ---
  {Nb}/T-bone-V           T-bone-V                  ---

  {Mo}/Liver              Blood1                    1.39
  {Mo}/UrinaryPath        Blood1                    1.39
  {Mo}/OtherKidneyTis     Blood1                    1.39
  {Nb}/Liver0             Liver0                    ---
  {Nb}/Liver1             Liver1                    ---
  {Nb}/Kidneys            Kidneys                   ---

  {Mo}/OtherTissue        Blood1                    1.39
  {Nb}/ST0                ST0                       ---
  {Nb}/ST1                ST1                       ---

$endif
