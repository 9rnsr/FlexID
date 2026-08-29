# ICRP Publ.134 p.224 Para.484
# > (484) The model for rubidium as a progeny of strontium is a condensed version of
# > a proposed model for rubidium as a parent radionuclide (Leggett and Williams,
# > 1988). The model is based on the same principles as the model for caesium, a chemical
# > and physiological analogue of rubidium, described elsewhere in the OIR series.
# > That is, the biokinetics of systemic rubidium is predicted on the basis of the distribution
# > of cardiac output, experimentally determined tissue-specific extraction fractions,
# > and the steady-state distribution of stable rubidium in the body. The reference
# > division of cardiac output in the adult male tabulated in Publication 89 (ICRP, 2002)
# > is applied here. ...
# > (484) ストロンチウムの娘核種としてのルビジウムに関するモデルは、親放射性核種としての
# > ルビジウムについて提案されたモデル（Leggett and Williams, 1988）を簡略化したものです。
# > このモデルは、OIRシリーズの他の箇所で記述されているルビジウムの化学的・生理学的類似体である
# > セシウムのモデルと同じ原理に基づいています。すなわち、全身におけるルビジウムの生物学的動態は、
# > 心拍出量の分配、実験的に決定された組織特異的な抽出率、および体内における安定ルビジウムの
# > 定常状態での分布に基づいて予測されます。ここでは、Publication 89（ICRP, 2002）に示された
# > 成人男性における心拍出量の基準分配が適用されます。 ...

[{Rb}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Plasma                Blood
  acc   RBC                   Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   Muscle                Muscle
  acc   OtherTissue           Other
  exc   Excreta(sweat)        ---


[{Rb}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.224 Para.484
# > (484) ... The present version of the model depicts blood plasma as a central
# > compartment that exchanges rubidium with RBCs, trabecular bone surface, cortical
# > bone surface, muscle, and a compartment representing all other soft tissue. Rates of
# > transfer of rubidium from plasma are as follows: 6 d⁻¹ to RBCs, 255 d⁻¹ to muscle,
# > 5.6 d⁻¹ to cortical bone surface, 8.4 d⁻¹ to trabecular bone surface, 855 d⁻¹ to other
# > tissue, 3.9 d⁻¹ to urinary bladder contents, 1.2 d⁻¹ to right colon contents, and
# > 0.1 d⁻¹ to excreta (loss in sweat). Transfer rates from RBCs or tissues to plasma
# > are as follows: 0.35 d⁻¹ from RBCs, 1.14 d⁻¹ from muscle, 1.68 d⁻¹ from bone surface
# > compartments, and 10.3 d⁻¹ from other tissue. ...
# > (484) ... 本モデルでは、血漿を中心コンパートメントとし、
# > そこから赤血球、海綿骨表面、皮質骨表面、筋肉、およびその他のすべての軟部組織を表す
# > コンパートメントとの間でルビジウムの交換が行われるものとしています。
# > 血漿から各コンパートメントへのルビジウムの移行速度は以下の通りです：
# > 赤血球へ6 d⁻¹、筋肉へ255 d⁻¹、皮質骨表面へ5.6 d⁻¹、海綿骨表面へ8.4 d⁻¹、その他の組織へ855 d⁻¹、
# > 膀胱内容物へ3.9 d⁻¹、右結腸内容物へ1.2 d⁻¹、および排泄物（汗による損失）へ0.1 d⁻¹。
# > 赤血球または各組織から血漿への移行速度は以下の通りです：赤血球から0.35 d⁻¹、
# > 筋肉から1.14 d⁻¹、骨表面コンパートメントから1.68 d⁻¹、およびその他の組織から10.3 d⁻¹。

  Plasma                  RBC                       6
  Plasma                  Muscle                  255
  Plasma                  C-bone-S                  5.6
  Plasma                  T-bone-S                  8.4
  Plasma                  OtherTissue             855
  Plasma                  UB-con                    3.9
  Plasma                  RC-con                    1.2
  Plasma                  Excreta(sweat)            0.1
  RBC                     Plasma                    0.35
  Muscle                  Plasma                    1.14
  C-bone-S                Plasma                    1.68
  T-bone-S                Plasma                    1.68
  OtherTissue             Plasma                   10.3

# ICRP Publ.134 p.224 Para.484
# > (484) ... Rubidium produced by decay of
# > strontium in blood is assigned to plasma. Rubidium produced in exchangeable or
# > non-exchangeable bone volume compartments of the strontium model are transferred
# > to plasma at the rate of bone turnover. Rubidium produced in soft tissue
# > compartments of the strontium model (ST0, ST1, or ST2) are transferred to
# > plasma at a rate of 10.3 d⁻¹. The subsequent behaviour of rubidium that reaches
# > plasma is determined by the model for rubidium described above.
# > (484) ... 血液中のストロンチウムの崩壊により生成されたルビジウムは血漿に移行します。
# > ストロンチウムモデルの交換性または非交換の骨体積コンパートメントで生成された
# > ルビジウムは、骨代謝の速度で血漿へ移行します。ストロンチウムモデルの軟部組織コンパートメント
# > （ST0、ST1、またはST2）で生成されたルビジウムは、10.3 d⁻¹の速度で血漿へ移行します。
# > 血漿に到達したルビジウムのその後の挙動は、上記のルビジウムモデルによって決定されます。

  {Sr}/Blood              Plasma                    ---
  {Rb}/Plasma             Plasma                    ---
  {Rb}/RBC                RBC                       ---

  {Sr,Rb}/C-bone-S        C-bone-S                  ---
  {Sr,Rb}/T-bone-S        T-bone-S                  ---
  {Sr}/Exch-C-bone-V      Plasma                $( 3% / 365)        # 骨ターンオーバー
  {Sr}/Exch-T-bone-V      Plasma                $(18% / 365)        # 骨ターンオーバー
  {Sr}/Noch-C-bone-V      Plasma                $( 3% / 365)        # 骨ターンオーバー
  {Sr}/Noch-T-bone-V      Plasma                $(18% / 365)        # 骨ターンオーバー

  {Rb}/Muscle             Muscle                    ---
  {Rb}/OtherTissue        OtherTissue               ---
  {Rb}/Excreta(sweat)     Excreta(sweat)            ---

  {Sr}/ST0                Plasma                   10.3
  {Sr}/ST1                Plasma                   10.3
  {Sr}/ST2                Plasma                   10.3
