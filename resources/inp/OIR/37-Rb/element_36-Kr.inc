[{Kr}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  exc   Exhaled               ---


[{Kr}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.206 Para.412
# > (412) Progeny of rubidium addressed in this publication are radioisotopes of
# > rubidium, krypton, and strontium. The model for rubidium as a parent is applied
# > to rubidium as a progeny of rubidium. Krypton produced in a tissue compartment
# > is assumed to transfer to blood with a half-time of 15 min and to be removed from
# > blood to the environment (exhaled) at a rate of 1000 d⁻¹. For application to strontium
# > as a progeny of rubidium, the characteristic model for strontium (ICRP,
# > 2016) was modified to address explicitly each of the tissues addressed in the
# > model for rubidium (see Annex B). The following transfer coefficients from compartments
# > of the rubidium model to the central blood compartment of the strontium
# > model were added to the characteristic model for strontium: RBC, 1000 d⁻¹;
# > kidneys, 0.116 d⁻¹; liver, 0.116 d⁻¹; muscle, 0.116 d⁻¹; red marrow, 0.116d⁻¹; and
# > rubidium’s ‘Other’, 2.5 d⁻¹. The following transfer coefficients from blood were
# > also added to the strontium model: kidneys, 0.00766 d⁻¹; liver, 0.0445 d⁻¹, muscle,
# > 0.716 d⁻¹; and red marrow, 0.0289 d⁻¹. The transfer coefficient from blood to the
# > intermediate-term soft tissue compartments of the strontium model was reduced
# > from 1.5 d⁻¹ to 0.703 d⁻¹ to leave the total outflow rate of strontium from blood at
# > 15 d⁻¹.
# > (412) 本書で扱うルビジウムの娘核種は、ルビジウム、クリプトン、およびストロンチウムの
# > 放射性同位体です。親核種としてのルビジウムに適用されるモデルは、ルビジウムの娘核種
# > としてのルビジウムに対しても適用されます。組織コンパートメント内で生成されたクリプトンは、
# > 15分の半減期で血液へ移行し、その後、血液から環境中へ（呼気として）1000 d⁻¹の速度で
# > 除去されると仮定されます。ルビジウムの娘核種であるストロンチウムに適用するにあたっては、
# > ストロンチウムの基本モデル（ICRP, 2016）を修正し、ルビジウムのモデルで扱われている
# > 各組織を明示的に取り込めるようにされました（付録Bを参照）。ストロンチウムの基本モデルには、
# > ルビジウムモデルの各コンパートメントからストロンチウムモデルの血液コンパートメント
# > （中心コンパートメント）への移行係数として、赤血球（RBC）：1000 d⁻¹、腎臓：0.116 d⁻¹、
# > 肝臓：0.116 d⁻¹、筋肉：0.116 d⁻¹、赤色骨髄：0.116 d⁻¹、およびルビジウムモデルにおける
# > 「その他（Other）」：2.5 d⁻¹が追加されました。また、血液から各組織への移行係数として、
# > 腎臓：0.00766 d⁻¹、肝臓：0.0445 d⁻¹、筋肉：0.716 d⁻¹、赤色骨髄：0.0289 d⁻¹が
# > ストロンチウムモデルに追加されました。さらに、血液からストロンチウムモデルの中期軟部組織
# > コンパートメントへの移行係数は、血液からのストロンチウムの総流出速度が15 d⁻¹となるよう、
# > 1.5 d⁻¹から0.703 d⁻¹へと変更されました。

  {Rb}/Plasma             Blood                     ---
  {Rb}/RBC                Blood                  1000
  {Kr}/Blood              Blood                     ---

  {Rb}/C-bone-S           Blood                 $(0.693 / (15/60/24))
  {Rb}/T-bone-S           Blood                 $(0.693 / (15/60/24))
  {Rb}/RedMarrow          Blood                 $(0.693 / (15/60/24))
  {Rb}/Liver              Blood                 $(0.693 / (15/60/24))
  {Rb}/Kidneys            Blood                 $(0.693 / (15/60/24))
  {Rb}/Muscle             Blood                 $(0.693 / (15/60/24))
  {Rb}/Other              Blood                 $(0.693 / (15/60/24))
  {Kr}/Exhaled            Exhaled                   ---

  Blood                   Exhaled                1000
