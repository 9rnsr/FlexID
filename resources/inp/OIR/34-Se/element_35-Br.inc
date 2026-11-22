[{Br}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   Liver                 Liver         # for progeny of Se
  acc   Kidneys               Kidneys       # for progeny of Se
  acc   Pancreas              Pancreas      # for progeny of Se
  acc   Spleen                Spleen        # for progeny of Se
  acc   Testes                Testes        # for progeny of Se
  acc   Ovaries               Ovaries       # for progeny of Se
  acc   Other                 Other


[{Br}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.199 Table 21.4
# Blood                   Other                   200       # fix
  Blood                   UB-con                    0.83
  Blood                   RC-con                    0.0083
  Other                   Blood                    15

# ICRP Publ.151 p.192 Para.390
# > (390) Progeny of selenium addressed in this publication are isotopes of selenium,
# > krypton, bromine, and arsenic. The model for selenium as a parent is applied to
# > selenium as a progeny of selenium. Krypton produced in a tissue compartment is
# > assumed to transfer to blood with a half-time of 15 min, and to be removed from
# > blood to the environment (exhaled) at a rate of 1000 d⁻¹. For application to bromine
# > as a progeny of selenium, the characteristic model for bromine was expanded to
# > include explicitly all tissues addressed in the selenium model. Deposition fractions
# > for the tissues added to the bromine model were based on the mass fractions (of total
# > body) of these tissues in the adult male. The assigned rates of transfer of bromine
# > from blood to added tissues are as follows: liver, 5.4 d⁻¹; kidneys, 0.94 d⁻¹; pancreas,
# > 0.42 d⁻¹; spleen, 0.45 d⁻¹; testes, 0.1 d⁻¹; and ovaries, 0.033 d⁻¹. The transfer rate
# > from blood to bromine’s ‘Other’ was reduced to 192.66 d⁻¹ to leave the total outflow
# > rate from blood unchanged. A removal half-time to blood of 15 d⁻¹ was assigned to
# > each of the added compartments. For an arsenic isotope produced in a compartment
# > not contained in the characteristic model for arsenic, the isotope was assumed to
# > transfer to the central blood compartment of the characteristic model for arsenic at a
# > rate of 1000 d⁻¹ if produced in a blood compartment and 0.6 d⁻¹ if produced in a
# > tissue compartment, and to follow the characteristic model for arsenic thereafter.
# > (390) 本稿で扱うセレンの娘核種には、セレン、クリプトン、臭素、およびヒ素の同位体が
# > 含まれまする。親核種としてのセレンに適用されるモデルは、セレンの娘核種であるセレンに対しても
# > 同様に適用されます。組織コンパートメント内で生成されたクリプトンは、半減期15分で血液へ移行し、
# > その後、血液から環境中へ（呼気として）1000 d⁻¹の速度で除去されると仮定されます。セレンの
# > 娘核種である臭素に適用する場合、臭素固有のモデルを拡張し、セレンモデルで扱われている
# > すべての組織を明示的に含めるようにしました。臭素モデルに追加された各組織への沈着割合は、
# > 成人男性におけるそれらの組織の（体重に対する）質量割合に基づいています。血液から追加された
# > 各組織への臭素の移行速度は、肝臓：5.4 d⁻¹、腎臓：0.94 d⁻¹、膵臓：0.42 d⁻¹、脾臓：0.45 d⁻¹、
# > 精巣：0.1 d⁻¹、卵巣：0.033 d⁻¹と設定されました。血液からの総流出速度を維持するため、
# > 血液から臭素モデルにおける「その他（Other）」コンパートメントへの移行速度は192.66 d⁻¹に
# > 調整されました。追加された各コンパートメントから血液への除去については、半減期に対応する
# > 速度（15 d⁻¹に相当）を設定されました。ヒ素固有のモデルに含まれていないコンパートメント内で
# > ヒ素同位体が生成された場合、その同位体は、血液コンパートメント内で生成された場合は1000 d⁻¹の
# > 速度で、組織コンパートメント内で生成された場合は0.6 d⁻¹の速度で、ヒ素固有モデルの中心的な
# > 血液コンパートメントへ移行し、その後はヒ素固有モデルに従うものと仮定されます。

  {Se}/Plasma             Blood                     ---
  {Se}/RBC                Blood                  1000
  {Se}/C-bone-S           Blood                    15
  {Se}/T-bone-S           Blood                    15
  {Se}/Liver              Liver                     ---
  {Se}/Kidneys            Kidneys                   ---
  {Se}/Pancreas           Pancreas                  ---
  {Se}/Spleen             Spleen                    ---
  {Se}/Testes             Testes                    ---
  {Se}/Ovaries            Ovaries                   ---
  {Se}/ST1                Blood                    15
  {Se}/ST2                Blood                    15

  Blood                   Liver                     5.4
  Blood                   Kidneys                   0.94
  Blood                   Pancreas                  0.42
  Blood                   Spleen                    0.45
  Blood                   Testes                    0.1
  Blood                   Ovaries                   0.033
  Blood                   Other                   192.66
  Liver                   Blood                    15
  Kidneys                 Blood                    15
  Pancreas                Blood                    15
  Spleen                  Blood                    15
  Testes                  Blood                    15
  Ovaries                 Blood                    15
