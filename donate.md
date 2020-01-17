---
layout: page
title: TABS.DONATE
---

{% tf donate.md %}

<article class="donate">
    <section style="display:flex;flex-direction:column;">
        <script src="https://donorbox.org/widget.js" paypalExpress="false"></script><iframe allowpaymentrequest="" frameborder="0" height="900px" name="donorbox" scrolling="no" seamless="seamless" src="https://donorbox.org/embed/spytify" style="width: auto; max-height:none!important"></iframe>
    </section>
    <section>
        <h3>{% t DONATE.DONORS %}</h3>
        <ol>
            {% for donor in site.data.donors %}
                {% if forloop.index <= 1 %}
                    <li style="color:#1E963C;"><strong>{{ donor.name }}</strong> - <em>${{ donor.donation}}</em></li>
                {% elsif forloop.index <= 3 %}
                    <li><strong>{{ donor.name }}</strong> - <em>${{ donor.donation}}</em></li>
                {% elsif forloop.index <= 10 %}
                    <li>{{ donor.name }} - <em>${{ donor.donation}}</em></li>
                {% endif %}
            {% endfor %}
        </ol>
        {% for donor in site.data.donors %}
        <!-- If I reached over 10 donors -->
            <!-- {% if forloop.index > 100 %}
                {% if forloop.last %}<span style="font-size:90%;color:#ccc;">...</span>{% endif %}
            {% elsif forloop.index > 80 %}
                <span style="font-size:90%;color:#ccc;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% elsif forloop.index > 50 %}
                <span style="font-size:90%;color:#aaa;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% elsif forloop.index > 20 %}
                <span style="font-size:90%;color:#888;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% elsif forloop.index > 10 %}
                <span style="font-size:90%;color:#666;">{{ donor.name }}{% if forloop.last == false %},{% endif %}</span>
            {% endif %} -->
        {% else %}
            <pre>{% t DONATE.NO_DONORS %}</pre>
        {% endfor %}
    </section>
</article>

