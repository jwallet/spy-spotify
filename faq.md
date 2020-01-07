---
layout: page
title: TABS.FAQ
---

{% for question in site.faq %}
<section id="{{ question.hash }}">
    <h3><a href="#{{question.hash}}">{% t {{ question.title  }} %}</a></h3>
    <p>{{ question.content }}</p>
</section>
{% endfor %}