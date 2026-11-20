[{As}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
  acc   Other1                Other
  acc   Other2                Other
  acc   Other3                Other


[{As}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.183 Table 19.3
  Plasma                  RBC                       2.0
  Plasma                  Other1                   20
  Plasma                  Other2                    1.52
  Plasma                  Other3                    0.28
  Plasma                  Liver1                    2.4
  Plasma                  Kidneys1                  2.52
  Plasma                  Kidneys2                  0.28
  Plasma                  C-bone-S                  1.0
  Plasma                  T-bone-S                  1.0
  Plasma                  UB-con                    8.4
  Plasma                  RC-con                    0.6
  RBC                     Plasma                    0.3
  Other1                  Plasma                    0.6
  Other2                  Plasma                    0.08
  Other3                  Plasma                    0.018
  Liver1                  Plasma                    0.95
  Liver1                  Liver2                    0.05
  Liver2                  Plasma                    0.07
  Kidneys1                UB-con                    5.0
  Kidneys2                Plasma                    0.7
  C-bone-S                Plasma                    0.6
  T-bone-S                Plasma                    0.6
  C-bone-S                C-bone-V                  0.003
  T-bone-S                T-bone-V                  0.006
  C-bone-V                Plasma                    0.0000821
  T-bone-V                Plasma                    0.000493

# ICRP Publ.151 p.173 Para.343
# > (343) Progeny of germanium addressed in this publication are radioisotopes of
# > gallium and arsenic. The models for gallium and arsenic as germanium progeny are
# > expansions of the characteristic models for these elements with added compartments
# > and associated transfer coefficients needed to solve the linked biokinetic
# > models for chains headed by germanium (see Annex B). If produced in a compartment
# > not explicitly named in the progeny’s model, the progeny is assumed to
# > transfer to the central blood compartment of its characteristic biokinetic model,
# > and to follow that model thereafter. The assigned transfer rate to the central blood
# > compartment is the rate of bone turnover for the indicated bone type if the progeny
# > is produced in a bone volume compartment, and at the following elementspecific
# > rate if produced in any other ambiguous compartment: gallium, 1.39 d⁻¹;
# > and arsenic, 0.6 d⁻¹.
# > (343) 本書で扱うゲルマニウムの娘核種は、ガリウムおよびヒ素の放射性同位体である。
# > ゲルマニウムの娘核種としてのガリウムおよびヒ素に関するモデルは、各元素固有のモデルを
# > 拡張したものである。これには、ゲルマニウムを起点とする崩壊系列の連鎖的な生物動態モデルを
# > 解くために必要な、コンパートメントの追加およびそれに関連する移行係数が含まれている
# > （付録Bを参照）。娘核種が、そのモデル内で明示的に指定されていないコンパートメントで
# > 生成された場合、その娘核種は、当該元素固有の生物動態モデルにおける中心血液コンパートメントへ
# > 移行し、その後はそのモデルに従うものと仮定する。中心血液コンパートメントへの移行速度は、
# > 娘核種が骨体積コンパートメントで生成された場合には、当該骨種における骨代謝回転速度とする。
# > それ以外の、明確に定義されていないコンパートメントで生成された場合には、元素ごとに
# > 以下の移行速度を適用する：ガリウムについては1.39 d⁻¹、ヒ素については0.6 d⁻¹とする。

  {Ge}/Blood              Plasma                 1000

  {Ge}/C-bone-S           C-bone-S                  ---
  {Ge}/T-bone-S           T-bone-S                  ---
  {Ge}/C-bone-V           C-bone-V                  ---
  {Ge}/T-bone-V           T-bone-V                  ---

  {Ge}/Liver              Plasma                    0.6
  {Ge}/Kidneys            Plasma                    0.6

  {Ge}/Other              Plasma                    0.6
