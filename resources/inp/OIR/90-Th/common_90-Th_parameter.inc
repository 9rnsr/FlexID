[parameter]

# ICRP Publ.137 p.231 Para.484 / 鉛子孫元素としてのポロニウム
# > (484) ... polonium as a progeny transferred to blood from either the respiratory
# > or alimentary tract is assigned to Plasma 2. ...
# > (484) ... 呼吸器系または消化器系から血液に移行する子孫核種としてのポロニウムは、
# > 血漿2に割り当てられます。 ...

  ${Po}/Blood               = 'Plasma2'

  ${U,Bi,Pb,Tl}/Blood       = 'Plasma'
  ${Pa,Th,Ac,Ra,Rn}/Blood   = 'Blood'

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
