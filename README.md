
# Ember+ control protocol - Slick and free for all! #

## Status
[![Build Status](https://travis-ci.org/Lawo/ember-plus.svg?branch=master)](https://travis-ci.org/Lawo/ember-plus)

## Introduction

The topic of control protocols keeps manufacturers and system integrators busy for years. _Ember+_ is an initiative out of the [Lawo Group](http://www.lawo.com/) that was started, because even for our small group of companies it was well worth defining a control protocol, which allows all of our equipment to interact without significant development effort.

We have decided to make this protocol openly available to everyone interested. This developer website gives public access to everything needed to get up to speed with _Ember+_: documentation, software libraries in various programming languages and platforms, source code and binaries. Our regular releases also include helpful examples and tools, allowing to implement _Ember+_ even into many existing products within a very short time.

The focus of Ember+ was set to fulfill the following requirements:
  * Easy to understand and implement for new interested programmers
  * Minimal hardware requirements for controlled devices (Ember+ provider)
  * Possibility to be used on a wide range of hardware platforms, from basic micro controllers all the way up to powerful PCs
  * Minimal development effort required to control new unknown devices once Ember+ is implemented on a product

Some further explanation in regards to the latter point: We've experienced that most control protocols are defined in a relatively narrow way and/or are very specific to audio. Some seem even to compromise technical specification of products - maybe dictating properties of parameters, ranges or resolutions. However, most protocols feature proprietary extensions. We have found that in the real world manufacturers are using these options extensively. Therefore we decided that the Ember+ protocol doesn't standardize parameters of devices at all. In fact it can be compared to an approach like XML has chosen. We define the communication parts and a basic data tree structure, but not the content of it. We believe that implementing parameters of a new device is not the job which takes a lot of effort. But it is often the cause of endless discussions in regards to standardization.

Therefore: Please note the possibility in Ember+ of defining a "schema" within your Ember+ tree. If you use this feature, I'd like to encourage you to publish your schema (type information about a data subtree) on the wiki part of this website. Just register and contribute it to the discussion forum.

Feel free to implement Ember+ in your devices. It's there now, the base of it has been used for years successfully. Please feel free to download the latest package of information in the download section of this developer site.

[Here is a list of companies that are already using Ember+.](../../wiki/Companies)

Enjoy!

# Contents of this SDK

The complete documentation of the Ember+ protocol stack may be found in the 'documentation' directory.

In addition this SDK contains the following components in source code form:

  * EmberLib.Net
    A .Net implementation of the Ember+ protocol with additional helpers and
    numerous example applications demonstrating its use.

  * libember_slim
    An ANSI C implementation of the Ember+ protocol usable as a static
    library or as a shared library.

  * libember
    A standard C++03 implementation of the Ember+ protocol usable as a static
    library, a shared library or as a header only library.

  * libs101
    A standard C++03 implementation of the s101 framing used by Ember+.

  * libformula
    A standard C++03 implementation of an evaluator for UPN expressions as
    used by Ember+.

  * tinyember/TinyEmberPlus
    A libember, libs101 & libformula based generic Ember+ provider.

  * tinyember/TinyEmberPlusRouter
    A libember, libs101 & libformula based Ember+ provider demonstrating the
    use of the specialized matrix extensions.

This SDK also contains a premake4 project file that may be used to generate native project files for various platforms and development environments for the 'libember' and 'libember_slim' projects.
