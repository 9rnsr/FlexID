[{C}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   BloodM                Blood         # Methane model
  acc   Blood0                Blood         # Generic model
  acc   Blood1                Blood         # CO2 model
  acc   Blood2                Blood         # CO2 model
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   Sys-Tis(short)        Other
  acc   Sys-Tis(long)         Other
  acc   ST0                   Other
  acc   ST1                   Other
  acc   ST2                   Other
  acc   ST3                   Other
  exc   Exhalation            ---
  exc   Excreta(skin)         ---


[{C}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.66 Table 3.5
  Blood1                  Exhalation               36.2
  Blood1                  Excreta(skin)             0.3
  Blood1                  UB-con                    0.65
  Blood1                  RC-con                    0.15
  Blood1                  ST0                      60
  Blood1                  ST1                       1.8
  Blood1                  ST2                       0.3
  Blood1                  ST3                       0.44
  Blood1                  T-bone-S                  0.09
  Blood1                  C-bone-S                  0.06
  Blood1                  T-bone-V                  0.006
  Blood1                  C-bone-V                  0.004
  ST0                     Blood1                   49.91
  ST1                     Blood1                    1.331
  ST2                     Blood1                    0.2218
  ST3                     Blood1                    0.01664
  ST1                     Blood2                    0.05545
  ST2                     Blood2                    0.009242
  ST3                     Blood2                    0.0006931
  Blood2                  UB-con                 1000
  T-bone-S                Blood1                    0.01733
  C-bone-S                Blood1                    0.01733
  T-bone-V                Blood1                    0.000493
  C-bone-V                Blood1                    0.0000821

# ICRP Publ.134 p.71 Para.139
# > (139) The model for systemic methane used in this publication is a modification of
# > the generic model for carbon described above. The generic model structure (Fig. 3.2)
# > is modified by addition of a blood compartment that receives methane absorbed
# > from the respiratory tract. It is assumed that the carbon label leaves this blood
# > compartment with a half-time of 10 min (removal rate of 100 d1), with 70%
# > removed in expired air and 30% entering the short-term tissue compartment of
# > the generic model structure. The parameter values of the generic model for carbon
# > are modified for application to methane to depict faster removal from the short-term
# > tissue compartment, lower transfer from the short-term tissue compartment to the
# > carbon dioxide model, and lower transfer from blood to the long-term tissue compartment
# > than predicted by the transfer coefficients listed in Table 3.6. It is assumed
# > for application to methane that carbon is removed from the short-term compartment
# > in Fig. 3.2 with a half-time of 1 d, with 60% going to the blood compartment of the
# > generic model structure, 30% going to the right colon contents, and 10% entering the
# > blood compartment of the carbon dioxide model. It is assumed that carbon leaves
# > the blood compartment of the generic model structure at a rate of 3 d1, with 3%
# > going to the long-term tissue compartment, 47% to the short-term retention compartment,
# > and 50% to the urinary bladder contents. As in the generic model, carbon
# > transfers from the long-term tissue compartment to the blood compartment of the
# > carbon dioxide model with a half-time of 70 d. The rate of production of carbon
# > dioxide from absorbed methane based on these modified parameter values is reasonably
# > consistent with the rate observed in sheep in the study by Dougherty et al.
# > (1967). The model predicts whole-body retention of carbon that is moderately
# > higher than observed in rats over the first 14 d after inhalation of labelled methane
# > (Didychuk et al., 2014).
# > (139) 本稿で用いる全身のメタンに関するモデルは、前述した炭素の汎用モデルを
# > 修正したものである。この汎用モデルの構造（図3.2）に対し、呼吸器系から吸収された
# > メタンを受け入れる「血液コンパートメント」を追加する形で修正を加えている。
# > この血液コンパートメントからは、半減期10分（消失速度100 d⁻¹）で炭素標識が消失すると
# > 仮定する。その内訳は、70%が呼気として排出され、30%が汎用モデル構造における
# > 「短期組織コンパートメント」へ移行する。メタンに適用するために、炭素の汎用モデルの
# > パラメータ値を以下のように修正した。すなわち、短期組織コンパートメントからの消失を
# > より速くし、短期組織コンパートメントから二酸化炭素モデルへの移行を抑え、さらに血液から
# > 「長期組織コンパートメント」への移行を、表3.6に示す移行係数による予測値よりも低く設定した。
# > メタンへの適用にあたっては、図3.2における短期コンパートメントから炭素が半減期1日で
# > 消失すると仮定する。その内訳は、60%が汎用モデル構造の血液コンパートメントへ、
# > 30%が右結腸の内容物へ、10%が二酸化炭素モデルの血液コンパートメントへと移行する。
# > また、汎用モデル構造の血液コンパートメントからは、速度3 d⁻¹で炭素が消失し、
# > その3%が長期組織コンパートメントへ、47%が「短期保持コンパートメント」へ、
# > 50%が膀胱内容物へと移行すると仮定する。汎用モデルと同様に、長期組織コンパートメントから
# > 二酸化炭素モデルの血液コンパートメントへは、半減期70日で炭素が移行する。これらの
# > 修正されたパラメータ値に基づき算出される、吸収されたメタンからの二酸化炭素生成速度は、
# > Doughertyら（1967）の研究においてヒツジで観察された速度と概ね一致している。本モデルによる
# > 全身の炭素保持量の予測値は、標識メタンの吸入後14日間において、ラットで観察された値よりも
# > やや高いものとなっている（Didychuk et al., 2014）。

  BloodM                  Exhalation            $(100 * 70%)
  BloodM                  Sys-Tis(short)        $(100 * 30%)

  Blood0                  Sys-Tis(short)        $(3 * 47%)
  Blood0                  Sys-Tis(long)         $(3 *  3%)
  Blood0                  UB-con                $(3 * 50%)
  Sys-Tis(short)          Blood1                $(0.693 * 10%)  # (半減期1日 = ln(2)/1)の10%
  Sys-Tis(short)          Blood0                $(0.693 * 60%)  # (半減期1日 = ln(2)/1)の60%
  Sys-Tis(short)          RC-con                $(0.693 * 30%)  # (半減期1日 = ln(2)/1)の30%
  Sys-Tis(long)           Blood1                    0.0099      # 半減期70日
