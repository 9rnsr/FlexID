[parameter]

  ${Fm,Es,Cf,Bk,Cm,Am,Np,Pa,Th,Ac,Ra,Rn}/Blood  = 'Blood'

# ICRP Publ.141 p.236 Para.567
# > (567) The model for plutonium as a progeny is further modified by removing the
# > transfers from blood to ST0 and from blood to Blood 1 (Table 18.6), and adding a
# > transfer of 0.33 d⁻¹ from Blood 1 to ST0. This simplifies the model for plutonium as
# > a progeny by eliminating a blood compartment (‘Blood 0’ in Table 18.6). ...
# > (567) 娘核種としてのプルトニウムに関するモデルは、血液からST0への移行および血液から
# > Blood 1への移行（表18.6）を削除し、Blood 1からST0への0.33 d⁻¹の移行を追加することに
# > よって、さらに修正されます。これにより、血液コンパートメント（表18.6における「Blood 0」）が
# > 排除され、娘核種としてのプルトニウムのモデルが簡素化されます。 ...
$if parent
  ${Pu}/Blood   = 'Blood'
$else
  ${Pu}/Blood   = 'Blood1'
$endif

  ${U,Bi,Pb,Tl}/Blood   = 'Plasma'

# ICRP Publ.137 p.231 Para.484 / 鉛子孫元素としてのポロニウム
# > (484) ... polonium as a progeny transferred to blood from either the respiratory
# > or alimentary tract is assigned to Plasma 2. ...
# > (484) ... 呼吸器系または消化器系から血液に移行する子孫核種としてのポロニウムは、
# > 血漿2に割り当てられます。 ...
$if parent
  ${Po}/Blood   = 'Plasma1'
$else
  ${Po}/Blood   = 'Plasma2'
$endif

  ${Fm}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.454 Table 28.2  / Ingested materials, All compounds
  ${Es}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.446 Table 27.2  / Ingested materials, All compounds
  ${Cf}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.434 Table 26.2  / Ingested materials, All compounds
  ${Bk}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.424 Table 25.2  / Ingested materials, All compounds
  ${Cm}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.408 Table 24.2  / Ingested materials, All compounds
  ${Am}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.382 Table 23.5  / Ingested materials, All compounds
  ${Pu}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.337 Table 22.11 / Ingested materials, Soluble forms
  ${Np}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.262 Table 21.2  / Ingested materials, All chemical forms
  ${U}/fA_MaxValueOfIng  = 0.02         # ICRP Publ.137 p.409 Table 15.2  / Ingested materials, Soluble forms
  ${Pa}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.251 Table 20.2  / Ingested materials, All forms
  ${Th}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.137 p.362 Table 14.2  / Ingested materials, All forms
  ${Ac}/fA_MaxValueOfIng = 5E-4         # ICRP Publ.141 p.240 Table 19.2  / Ingested materials, All compounds
  ${Ra}/fA_MaxValueOfIng = 0.2          # ICRP Publ.137 p.325 Table 13.2  / Ingested materials, All forms
  ${Fr}/fA_MaxValueOfIng = 0.99 # ≒1   # ICRP Publ.151 p.363 Table 39.2  / Ingested materials, All forms
 #${Rn}
  ${At}/fA_MaxValueOfIng = 0.99 # ≒1   # ICRP Publ.151 p.356 Table 38.3  / Ingested materials, All forms
  ${Po}/fA_MaxValueOfIng = 0.1          # ICRP Publ.137 p.279 Table 11.2  / Ingested materials, All chemical forms
  ${Bi}/fA_MaxValueOfIng = 0.05         # ICRP Publ.137 p.253 Table 10.2  / Ingested materials, All forms
  ${Pb}/fA_MaxValueOfIng = 0.2          # ICRP Publ.137 p.220 Table  9.2  / Ingested materials, All forms
  ${Tl}/fA_MaxValueOfIng = 0.99 # ≒1   # ICRP Publ.151 p.344 Table 37.2  / Ingested materials, All forms
