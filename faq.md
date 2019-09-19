---
layout: page
title: TABS.FAQ
---

{% for question in site.faq %}
<section>
    <h3>{% t {{ question.title  }} %}</h3>
    <p>{{ question.content }}</p>
</section>
{% endfor %}