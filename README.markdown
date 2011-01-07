Atom.NET.alt
===============================================================================
Atom.NET.alt は Atom.net の目コピフォークバージョンです。


修正内容
===============================================================================
修正内容は以下のとおりです。抜けているところがあるかもしれません。

* AtomContentConstruct クラスを AtomContentBase クラスに変更。
* AtomNSUri の中身を new Uri( "http://www.w3.org/2005/Atom" )に変更。
* Base64 に関するメソッドを Base64 クラスにまとめた。
* Utils クラスを AtomUtility クラスに変更。
* Mode 列挙体を EncodedMode 列挙体に変更。
* internal なメソッドを lowerCamelCase に変更。

TODO
===============================================================================
* AtomDateConstruct クラスの LocalName は modified じゃなくて、updated じゃない
  の？
* 名前をもとに戻すかどうか。
