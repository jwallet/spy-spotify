---
layout: page
title: F.A.Q.
---

{% for question in site.faq %}
<section id="{{ question.hash }}">
<details class="faq">
    <summary class="faq_title"><h3>{{ question.title }}</h3></summary>
    <p>{{ question.content }}</p>
</details>
</section>
{% endfor %}
